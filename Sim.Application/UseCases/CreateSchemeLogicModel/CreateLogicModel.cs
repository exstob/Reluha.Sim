using Sim.Application.NanoServices;
using Sim.Domain.Logic;
using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.UseCases.CreateLogicModel;


public interface ICreateLogicModel : IUseCase
{
    public LogicModel Generate(UiSchemeModel uiModel);
}

public class CreateLogicModel: ICreateLogicModel
{
    public LogicModel Generate(UiSchemeModel uiModel) 
    {
        var (relays, contacts) = Parser.Parse(uiModel);

        var model = new LogicModel(relays, contacts);

        return model;
    }
}

