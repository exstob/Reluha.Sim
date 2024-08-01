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
    static public SchemeLogicModel Generate(UiSchemeModel elements) 
    {
        var model = new SchemeLogicModel(Guid.NewGuid());

        return model;
    }
}

