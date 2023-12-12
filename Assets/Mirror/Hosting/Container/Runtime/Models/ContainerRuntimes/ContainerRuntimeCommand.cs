using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mirror.Hosting.Container.Runtime.Models.ContainerRuntimes
{
    public abstract class ContainerRuntimeCommand
    {
        public bool IsValidCommand { get; private set; }

        protected abstract string Sanitize(string rawCommandInput);
        protected abstract bool Validate(string sanitizedCommandInput);
        protected abstract IEnumerable<string> ParseArguments(string commandInput);
        private string sanitizedCommand;
        private readonly string commandUserInput;
        protected IEnumerable<string> commandArguments;
        protected ContainerRuntimeCommand(string commandUserInput)
        {
            this.commandUserInput = commandUserInput;
            Setup();
        }
        private void Setup()
        {
            sanitizedCommand = Sanitize(commandUserInput);
            IsValidCommand = Validate(sanitizedCommand);
            if (IsValidCommand == false)
                throw new Exception($"container hosting | invalid command | {this}");
            commandArguments = ParseArguments(sanitizedCommand);
        }
        public abstract Task<ContainerRuntimeCommandResponse> Execute();
        public override string ToString()
        {
            return sanitizedCommand;
        }
    }
}
