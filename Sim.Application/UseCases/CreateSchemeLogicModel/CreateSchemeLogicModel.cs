using Sim.Application.NanoServices;
using Sim.Domain.Logic;
using Sim.Domain.UiSchematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.UseCases.CreateSchemeLogicModel;

internal class CreateSchemeLogicModel
{
    static public SchemeLogicModel Generate(UiSchemeModel uiModel) 
    {
        var (relays, contacts) = Parser.Parse(uiModel);

        var model = new SchemeLogicModel(relays, contacts);

        return model;
    }
}

