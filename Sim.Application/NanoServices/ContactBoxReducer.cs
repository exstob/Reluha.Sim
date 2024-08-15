using Sim.Domain.ParsedScheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.NanoServices;

public static class ContactBoxReducer
{
    public static bool TryPackParallelContactBoxes(in List<ContactBox> inputBoxes, out List<ContactBox> outputBoxes)
    {
        List<ContactBox> boxes = inputBoxes;

        var parallelBoxes = boxes
            .GroupBy(p => new { p.FirstPin, p.SecondPin }) /// group boxes with same Node
            .Select(g => new ContactBox(ContactBoxType.Parallel) { FirstPin = g.Key.FirstPin, SecondPin = g.Key.SecondPin, Boxes = g.ToList() })
            .ToList();
        foreach (var parBox in parallelBoxes)
        {
            boxes.Add(parBox);
            boxes = boxes.Except(parBox.Boxes).ToList();
        }

        //var parallelBoxNodes = parallelBoxes.Select(box => box.FirstPin as Node).Concat(parallelBoxes.Select(box => box.SecondPin as Node)).ToList();
        var allBoxNodes = parallelBoxes.Select(box => box.FirstPin).Concat(parallelBoxes.Select(box => box.SecondPin)).ToList();

        foreach (var pb in parallelBoxes)
        {
            var pin1Intersections = allBoxNodes.Count(b => b.Equals(pb.FirstPin));
            if (pin1Intersections < 3 && pb.FirstPin is Node node1) /// it means we can simplified the node
            {
                pb.FirstPin = new Pin(node1.Id);
            }

            var pin2Intersections = allBoxNodes.Count(b => b.Equals(pb.SecondPin));
            if (pin2Intersections < 3 && pb.SecondPin is Node node2) /// it means we can simplified the node
            {
                pb.SecondPin = new Pin(node2.Id);
            }
        }

        outputBoxes = boxes;
        return parallelBoxes.Count > 0;
    }

    public static bool TryPackSerialContactBoxes(in List<ContactBox> inputBoxes, out List<ContactBox> outputBoxes)
    {
        bool found = false;
        List<ContactBox> boxes = inputBoxes;

        var edgePins = inputBoxes
            .SelectMany(ib => new[] { ib.FirstPin, ib.SecondPin })
            .Where(pin => pin is PolePositive || pin is PoleNegative || pin is RelayPlusPin || pin is RelayMinusPin || pin is Node)
            .ToList();

        foreach (var startPin in edgePins)
        {
            var (serialBoxes, lastPin) = TryFindSerialBoxes(startPin!, boxes);

            if (serialBoxes?.Count > 0)
            {
                found = true;
                var box = new ContactBox(ContactBoxType.Serial)
                {
                    FirstPin = startPin!,
                    SecondPin = lastPin,
                    Boxes = serialBoxes,
                };

                boxes.Add(box);
                boxes = boxes.Except(box.Boxes).ToList();
            }
        }

        outputBoxes = boxes;
        return found;
    }

    static (List<ContactBox> serialBoxes, ILogicEdge lastPin) TryFindSerialBoxes(ILogicEdge startPin, List<ContactBox> inputBoxes)
    {
        ILogicEdge pin = startPin;
        var startBox = inputBoxes.Single(ib => ib.FirstPin.Equals(startPin));
        var box = startBox;

        List<ContactBox> boxes = [];

        while (true)
        {
            var nextBox = inputBoxes.Find(ib => ib.FirstPin is Pin && ib.FirstPin.Equals(box.SecondPin));

            if (nextBox is null) goto FINISH; //// detect the end of serial chain.

            boxes.Add(nextBox);
            box = nextBox;
            pin = box.SecondPin;
        }

    FINISH:
        return boxes.Count > 0 ? ([startBox, .. boxes], pin) : ([], pin);

    }
}
