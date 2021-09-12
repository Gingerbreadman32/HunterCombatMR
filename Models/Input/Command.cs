namespace HunterCombatMR.Models.Input
{
    public struct Command
    {
        public Command(string name,
            BufferedInput[] inputs,
            FrameLength time,
            FrameLength buffer)
        {
            Name = name;
            Inputs = inputs;
            BufferTime = buffer;
            Time = time;
        }

        public FrameLength BufferTime { get; }
        public BufferedInput[] Inputs { get; }
        public string Name { get; }
        public FrameLength Time { get; }
    }
}