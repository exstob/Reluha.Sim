using Sim.Domain;
using Sim.Domain.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.UseCases.CreateSchemeLogicModel;

internal class CreateSchemeLogicModel
{
    static public SchemeLogicModel Generate(SchemeElements elements) 
    {
        var model = new SchemeLogicModel(Guid.NewGuid());

        return model;
    }
}

