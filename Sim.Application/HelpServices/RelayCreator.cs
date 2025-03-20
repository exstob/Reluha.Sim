using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Sim.Domain.Logic;
using Sim.Domain.ParsedSchema;
using Sim.Domain.ParsedScheme;
using Sim.Domain.UiSchematic;

namespace Sim.Application.NanoServices;

public class RelayCreator
{
    public static List<Relay> Create(UiSchemeModel elements, List<LogicBox> boxes)
    {
        List<Relay> relays = [];
        foreach (var iuRelay in elements.Relays)
        {
            var plusPin = boxes.Find(pin => pin.SecondPin is RelayPlusPin plusPin && plusPin.RelayName == iuRelay.Name);
            var minusPin = boxes.Find(pin => pin.SecondPin is RelayMinusPin minusPin && minusPin.RelayName == iuRelay.Name);

            if (plusPin != null && minusPin != null)
            {
                var relay = new Relay { Name = iuRelay.Name, State = new RelayState(plusPin.ToString(), minusPin.ToString()) };
                if (iuRelay.ExtraProps.MqttTopic is not null) 
                {
                    relay.Connection = new() { MqttTopic = iuRelay.ExtraProps.MqttTopic };
                }

                relays.Add(relay);
            }
            else
            { 
                /////TODO: add logs
            }
        }

        return relays;
    }

}
