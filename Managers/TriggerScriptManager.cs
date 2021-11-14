using HunterCombatMR.Constants;
using HunterCombatMR.Enumerations;
using HunterCombatMR.Interfaces;
using HunterCombatMR.Models;
using HunterCombatMR.Models.Trigger;
using HunterCombatMR.Parsers;
using HunterCombatMR.Services;
using HunterCombatMR.Triggers.Operators;
using HunterCombatMR.Triggers.Operators.Math;
using HunterCombatMR.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HunterCombatMR.Managers
{
    public sealed class TriggerScriptManager
        : ManagerBase
    {
        private static IEnumerable<FieldInfo> _componentFields;
        private static IEnumerable<IScriptParser> _scriptParsers;
        private static IEnumerable<IInternallyNamed> _triggerFunctions;

        private readonly IEnumerable<OperatorParser> _operatorParsers = new List<OperatorParser>()
        {
            new ComparativeOperatorParser("=", nameof(Equals)),
            new ComparativeOperatorParser("!=", nameof(NotEquals)),
            new BinaryOperatorParser("+", nameof(Add), 2),
            new BinaryOperatorParser("-", nameof(Subtract), 2),
            new BinaryOperatorParser("*", nameof(Multiply), 3),
            new BinaryOperatorParser("/", nameof(Divide), 3),
            new LeftEncapsulatingOperatorParser(),
            new LeftEncapsulatingOperatorParser("sin", nameof(Sine)),
            new LeftEncapsulatingOperatorParser("cos", nameof(Cosine)),
            new LeftEncapsulatingOperatorParser("max", nameof(Maximum)),
            new LeftEncapsulatingOperatorParser("min", nameof(Minimum)),
            new RightEncapsulatingOperatorParser()
        };

        public static IReadOnlyCollection<FieldInfo> ComponentFieldTriggerFunctions { get => _componentFields.ToList(); }

        public static TResult GetTriggerFunctionResult<TResult>(string functionName,
            int entityId,
            params ITriggerFunctionParameter[] parameters)
        {
            var function = GetFunction(functionName) as ITypedInvokedFunction<TResult>;

            if (function is null)
                throw new ArgumentOutOfRangeException($"Function provided provides incorrect type of result! Expected {typeof(TResult)}.");

            return function.Invoke(entityId, parameters);
        }

        public static object GetTriggerFunctionResult(string functionName,
            int entityId,
            Type resultType,
            params ITriggerFunctionParameter[] parameters)
        {
            var function = GetFunction(functionName) as ITypedInvokedFunction<TResult>;

            if (function is null)
                throw new ArgumentOutOfRangeException($"Function provided provides incorrect type of result! Expected {typeof(TResult)}.");

            return function.Invoke(entityId, parameters);
        }

        public static IEnumerable<TriggerFunctionElement<string>> ParseScript(string script,
            out TriggerFunctionElement<bool> resultFunction)
        {
            if (string.IsNullOrWhiteSpace(script))
                throw new ArgumentNullException("Script is null or empty!");

            script = ParseForOperators(script);
            string comparatorToken = script.Split().SingleOrDefault(x => IsOperator(x)
                && GetParser(x).Associativity.Equals(OperatorAssociativity.NonAssociative));

            if (comparatorToken is null)
                throw new FormatException("Trigger must have a single comparitive function!");

            IComparatorFunction comparator = GetFunction(GetParser(comparatorToken).FunctionName) as IComparatorFunction;

            if (comparator is null)
                throw new FormatException($"Function {comparator.Name} is not a comparator function!");

            Stack<Operand> leftSide = ShuntingYardAlgo(script.Substring(0, script.IndexOf(comparatorToken)));
            Stack<Operand> rightSide = ShuntingYardAlgo(script.Substring(script.IndexOf(comparatorToken) + comparatorToken.Length));
            var references = new Stack<TriggerFunctionElement<string>>();
            Stack<IScriptElement<string>> operandsLeft = CreateComparatorParameter(leftSide, ref references);
            Stack<IScriptElement<string>> operandsRight = CreateComparatorParameter(rightSide, ref references);

            resultFunction = new TriggerReference<bool>(comparator.GetType(), operandsLeft.Pop(), operandsRight.Pop());

            return references;
        }

        public static Stack<Operand> ShuntingYardAlgo(string input)
        {
            var operatorStack = new Stack<IScriptParser>();
            bool hasEquality = false;
            var outputStack = new Stack<Operand>();
            string[] pieces = input.Split(' ');

            for (int i = 0; i < pieces.Length; i++)
            {
                if (!IsOperator(pieces[i]))
                {
                    CheckValidOperandWrapper(pieces[i], outputStack.Push);
                    continue;
                }

                Associate(pieces[i], ref operatorStack, ref hasEquality, ref outputStack, GetParser(pieces[i]));
            }

            while (operatorStack.Count > 0)
            {
                if (operatorStack.Peek().Associativity.Equals(OperatorAssociativity.LeftParenthesis))
                    throw new ArgumentException($"Parenthesis mismatch at index {input.IndexOf(operatorStack.Peek().Operator)}!", nameof(input));

                var function = operatorStack.Pop();

                outputStack.Push(Operand.FunctionOperand(function.FunctionName));
            }

            return outputStack;
        }

        protected override void OnDispose()
        {
            _triggerFunctions = null;
            _triggerFunctions = null;
            _scriptParsers = null;
        }

        protected override void OnInitialize()
        {
            _triggerFunctions = ReflectionUtils.InstatiateTypesFromInterface<IInternallyNamed>();
            _scriptParsers = new List<IScriptParser>(_operatorParsers);
            _componentFields = ReflectionUtils.GatherConstantsFromStaticType(typeof(ComponentTriggerParams));
        }

        private static Stack<IScriptElement> CreateComparatorParameter(Stack<Operand> tokenList)
        {
            var instructions = new Stack<IScriptElement>();

            while (tokenList.Count() > 0)
            {
                var token = tokenList.Peek();
                if (!token.Category.Equals(OperandCategory.Function))
                {
                    instructions.Push(tokenList.Pop().Value);
                    continue;
                }

                var parser = _scriptParsers.Single(x => x.FunctionName.Equals(token.Value.Solve()));

                if (string.IsNullOrWhiteSpace(parser.FunctionName))
                    continue;

                var function = GetFunction(parser.FunctionName);

                if (function is IComparatorFunction)
                    throw new FormatException("Cannot have multiple comparator functions in one equation!");

                var parameters = GetParameters(ref instructions, function);
                var reference = new TriggerFunctionElement(function.Name, parameters.ToArray());

                instructions.Push(reference);
            }

            return instructions;
        }

        private static void Associate(string fullScript, ref Stack<IScriptParser> operatorStack, ref bool hasEquality, ref Stack<Operand> outputStack, IScriptParser parser)
        {
            switch (parser.Associativity)
            {
                case OperatorAssociativity.Function:
                case OperatorAssociativity.LeftParenthesis:
                    operatorStack.Push(parser);
                    break;

                case OperatorAssociativity.NonAssociative:
                    if (hasEquality)
                        throw new ArgumentException("Cannot have multiple non-associative operators in one statement!", nameof(fullScript));
                    operatorStack.Push(parser);
                    hasEquality = true;
                    break;

                case OperatorAssociativity.RightParenthesis:
                    RightParenthesisLogic(fullScript, ref operatorStack, ref outputStack, parser);
                    break;

                default:
                    if (operatorStack.Count > 0)
                    {
                        OperatorLogic(ref operatorStack, ref outputStack, parser);
                    }
                    operatorStack.Push(parser);
                    break;
            }
        }

        private static void CheckValidOperandWrapper(string input,
                                            Action<Operand> push)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;

            if (float.TryParse(input, out float number))
            {
                push(new Operand(number.ToString(), OperandCategory.Number));
                return;
            }

            if (_componentFields.Any(x => ((string)x.GetRawConstantValue()).Equals(input)))
            {
                push(new Operand(input, OperandCategory.Variable));
                return;
            }

            throw new FormatException($"Invalid script! Input {input} is not numeric nor represents a component field!");
        }

        private static void DivideTokens(ref string input, ref string element, string operatorKey)
        {
            int indexOfLast = 0;
            while (element.Contains(operatorKey) && element.Count() > 1)
            {
                int currentIndex = input.IndexOf(operatorKey, indexOfLast);
                input = input.Insert(currentIndex + operatorKey.Length, " ");
                input = input.Insert(currentIndex, " ");
                element = element.Remove(element.IndexOf(operatorKey), operatorKey.Length);
                indexOfLast = currentIndex + 2;
            }
        }

        public static IInternallyNamed GetFunction(string name)
        {
            var function = _triggerFunctions.SingleOrDefault(x => x.Name.Equals(name));

            if (function is null)
                throw new NullReferenceException($"There are no loaded functions that correspond to function name {function}!");

            return function;
        }

        private static IEnumerable<IScriptElement> GetParameters(ref Stack<IScriptElement<string>> operands, IInternallyNamed function)
        {
            var parameters = new List<IScriptElement<string>>();
            while (parameters.Count < function.RequiredArguments && operands.Count >= 1)
            {
                parameters.Add(operands.Pop());
            }
            return parameters;
        }

        private static IScriptParser GetParser(string token)
            => _scriptParsers.Single(x => x.Operator.Equals(token));

        private static bool IsOperator(string token)
                            => _scriptParsers.Select(x => x.Operator).Any(x => token.Equals(x));

        private static bool LeftParenthesisCheck(IScriptParser parser)
            => parser.Associativity.Equals(OperatorAssociativity.LeftParenthesis)
                || parser.Associativity.Equals(OperatorAssociativity.Function);

        private static void OperatorLogic(ref Stack<IScriptParser> operatorStack,
                    ref Stack<Operand> outputStack,
            IScriptParser parser)
        {
            var nextOp = operatorStack.Peek();
            while (operatorStack.Count > 0
                && !LeftParenthesisCheck(nextOp)
                && (nextOp.Precedence > parser.Precedence ||
                    (nextOp.Precedence.Equals(parser.Precedence)
                    && nextOp.Associativity.Equals(OperatorAssociativity.Left))))
            {
                outputStack.Push(Operand.FunctionOperand(operatorStack.Pop().FunctionName));
                if (operatorStack.Count > 0)
                    nextOp = operatorStack.Peek();
            }
        }

        private static string ParseForOperators(string input)
        {
            string element = input;
            var operators = new List<KeyValuePair<int, string>>();
            var parsers = _scriptParsers.Where(x => element.Contains(x.Operator)).OrderByDescending(x => x.Precedence);
            foreach (var parser in parsers)
            {
                if (element.IndexOf(parser.Operator).Equals(-1) || !parser.Validate(input, input.IndexOf(parser.Operator)))
                    continue;

                DivideTokens(ref input, ref element, parser.Operator);
            }

            DivideTokens(ref input, ref input, ",");

            return input;
        }

        private static void RightParenthesisLogic(string input,
            ref Stack<IScriptParser> operatorStack,
            ref Stack<Operand> outputStack,
            IScriptParser parser)
        {
            while (operatorStack.Count > 1 && !LeftParenthesisCheck(operatorStack.Peek()))
            {
                outputStack.Push(Operand.FunctionOperand(operatorStack.Pop().FunctionName));
            }

            if (operatorStack.Count <= 1 && !LeftParenthesisCheck(operatorStack.Peek()))
                throw new ArgumentException($"Parenthesis mismatch at index {input.IndexOf(parser.Operator)}!", nameof(input));

            if (operatorStack.Peek().Associativity.Equals(OperatorAssociativity.Function))
            {
                outputStack.Push(Operand.FunctionOperand(operatorStack.Pop().FunctionName));
                return;
            }

            operatorStack.Pop();
        }
    }
}