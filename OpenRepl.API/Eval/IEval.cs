using OpenRepl.API.Eval.ResultModels;
using System.Threading.Tasks;

namespace OpenRepl.API
{
    public interface IEval
    {
        Task<EvalResult> RunEvalAsync(string code);
    }
}
