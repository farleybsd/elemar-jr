using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks.Dataflow;

namespace Dataflow.Block.Livro;

public interface IDownloadStringPipe
{
    TransformBlock<string, string> Create(ExecutionDataflowBlockOptions executionOptions);
}

public interface ICreateWordListPipe
{
    TransformBlock<string, string[]> Create(ExecutionDataflowBlockOptions executionOptions);
}

public interface IFilterWordListPipe
{
    TransformBlock<string[], string[]> Create(ExecutionDataflowBlockOptions executionOptions);
}

public interface IFindReversedWordsPipe
{
    TransformManyBlock<string[], string> Create(ExecutionDataflowBlockOptions executionOptions);
}

public interface IPrintReversedWordsPipe
{
    ActionBlock<string> Create(ExecutionDataflowBlockOptions executionOptions);
}