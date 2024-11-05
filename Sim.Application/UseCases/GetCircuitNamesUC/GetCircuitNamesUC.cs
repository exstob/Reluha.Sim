using Microsoft.Extensions.Logging;
using Sim.Application.Dtos.Simulate;
using Sim.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.UseCases.GetCircuitsUC;

public interface IGetCircuitNamesUC : IUseCase
{
    public List<string> GetCircuits();
}

public class GetCircuitNamesUC(IRepository repo, ILogger<IUseCase> logger) : IGetCircuitNamesUC
{
    private readonly ILogger<IUseCase> _logger = logger;
    private readonly IRepository _repo = repo;

    public List<string> GetCircuits()
    {
        return _repo.GetUiSchemeNames();
    }
}
