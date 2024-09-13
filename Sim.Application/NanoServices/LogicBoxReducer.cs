using Sim.Domain.ParsedScheme;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sim.Application.NanoServices;

public static class LogicBoxReducer
{
    public static bool TryPackParallelContactBoxes(in List<LogicBox> inputBoxes, out List<LogicBox> outputBoxes)
    {
        var packed = TryPackParallelContactBoxesWithSameNodes(inputBoxes, out var parBoxes);
        packed |= TryPackParallelContactBoxesWithPoleAndNode(parBoxes, out outputBoxes);

        return packed;
    }

        public static bool TryPackParallelContactBoxesWithSameNodes(in List<LogicBox> inputBoxes, out List<LogicBox> outputBoxes)
    {
        List<LogicBox> boxes = inputBoxes;

        var parallelBoxes = boxes
            .Where(b => b.FirstPin is Node && b.SecondPin is Node)
            .GroupBy(b => new { b.FirstPin, b.SecondPin }) /// group boxes with same Node
            .Where(b => b.Count() > 1)
            .Select(g => new LogicBox(LogicBoxType.Parallel) { FirstPin = g.Key.FirstPin, SecondPin = g.Key.SecondPin, Boxes = g.ToList() })
            .ToList();
        foreach (var parBox in parallelBoxes)
        {
            boxes = boxes.Except(parBox.Boxes).ToList();
            boxes.Add(parBox);
        }

        foreach (var pb in parallelBoxes)
        {
            var pin1IntersectedWith1 = boxes.Where(b => b.FirstPin.Equals(pb.FirstPin)).ToList();
            var pin1IntersectedWith2 = boxes.Where(b => b.SecondPin.Equals(pb.FirstPin)).ToList();
            if ((pin1IntersectedWith1.Count + pin1IntersectedWith2.Count) < 3 && pb.FirstPin is Node node1) /// it means we can simplified the node
            {
                var pin = node1.RelayPin ?? new Pin(node1.Id);
                pin1IntersectedWith1.ForEach(box => box.FirstPin = pin);
                pin1IntersectedWith2.ForEach(box => box.SecondPin = pin);
            }


            var pin2IntersectedWith1 = boxes.Where(b => b.FirstPin.Equals(pb.SecondPin)).ToList();
            var pin2IntersectedWith2 = boxes.Where(b => b.SecondPin.Equals(pb.SecondPin)).ToList();
            if ((pin2IntersectedWith1.Count + pin2IntersectedWith2.Count) < 3 && pb.SecondPin is Node node2) /// it means we can simplified the node
            {
                var pin = node2.RelayPin ?? new Pin(node2.Id);
                pin2IntersectedWith1.ForEach(box => box.FirstPin = pin);
                pin2IntersectedWith2.ForEach(box => box.SecondPin = pin);
            }
        }

        outputBoxes = boxes;
        return parallelBoxes.Count > 0;
    }

    public static bool TryPackParallelContactBoxesWithPoleAndNode(in List<LogicBox> inputBoxes, out List<LogicBox> outputBoxes)
    {
        List<LogicBox> boxes = inputBoxes;

        var parallelBoxes = boxes
            .Where(b => b.FirstPin is IPoleEdge && b.SecondPin is Node)
            .GroupBy(p => new { p.SecondPin }) /// group boxes with same Node
            .Where(b => b.Count() > 1)
            .Select(g => new LogicBox(LogicBoxType.Parallel) { FirstPin = new Poles(), SecondPin = g.Key.SecondPin, Boxes = g.ToList() })
            .ToList();

        foreach (var parBox in parallelBoxes)
        {
            boxes = boxes.Except(parBox.Boxes).ToList();
            boxes.Add(parBox);
        }

        foreach (var pb in parallelBoxes)
        {
            var pin2IntersectedWith1 = boxes.Where(b => b.FirstPin.Equals(pb.SecondPin)).ToList();
            var pin2IntersectedWith2 = boxes.Where(b => b.SecondPin.Equals(pb.SecondPin)).ToList();
            if ((pin2IntersectedWith1.Count + pin2IntersectedWith2.Count) < 3 && pb.SecondPin is Node node2) /// it means we can simplified the node
            {
                var pin = node2.RelayPin ?? new Pin(node2.Id);
                pin2IntersectedWith1.ForEach(box => box.FirstPin = pin);
                pin2IntersectedWith2.ForEach(box => box.SecondPin = pin);
            }
        }

        outputBoxes = boxes;
        return parallelBoxes.Count > 0;
    }

    public static bool TryPackSerialContactBoxes(List<LogicBox> inputBoxes, out List<LogicBox> outputBoxes)
    {
        bool found = false;
        List<LogicBox> boxes = inputBoxes;

        var edgePins = inputBoxes
            .Select(ib => ib.FirstPin)
            .Where(pin => pin is IPoleEdge || pin is Node)
            .ToList();

        foreach (var startPin in edgePins)
        {
            var startBoxes = inputBoxes.FindAll(ib => ib.FirstPin.Equals(startPin));
            foreach (var startBox in startBoxes) 
            {
                var (serialBoxes, lastPin) = TryFindSerialBoxes(startBox, boxes);

                if (serialBoxes?.Count > 0)
                {
                    found = true;
                    var box = new LogicBox(LogicBoxType.Serial)
                    {
                        FirstPin = startPin!,
                        SecondPin = lastPin,
                        Boxes = serialBoxes,
                    };

                    boxes = boxes.Except(box.Boxes).ToList();
                    boxes.Add(box);
                }
            }
        }

        outputBoxes = boxes;
        return found;
    }

    static (List<LogicBox> serialBoxes, ILogicEdge lastPin) TryFindSerialBoxes(LogicBox startBox, List<LogicBox> inputBoxes)
    {
        ILogicEdge pin = startBox.SecondPin;
        var box = startBox;

        List<LogicBox> boxes = [];

        while (true)
        {
            //var rightNextBox = inputBoxes.Find(ib => ib.FirstPin is Pin && ib.FirstPin.Equals(box.SecondPin));
            //var rotatedNextBox = inputBoxes.Find(ib => ib.SecondPin is Pin && !ib.Equals(box) && ib.SecondPin.Equals(box.SecondPin));

            var rightNextBox = inputBoxes.Find(ib => pin is Pin currentPin && ib.FirstPin is Pin pin1 && currentPin.Id.Equals(pin1.Id));

            ///Unfortunately, we can not detect the right direction for nested parallel connection, so we need to "rotate" the logic box 
            var rotatedNextBox = inputBoxes.Find(ib => pin is Pin currentPin && ib.SecondPin is Pin pin2 && currentPin.Id.Equals(pin2.Id) && !ib.Equals(box));

            var nextBox = rightNextBox ?? rotatedNextBox;

            if (nextBox is null) goto FINISH; //// detect the end of serial chain.

            boxes.Add(nextBox);
            box = nextBox;
            pin = rightNextBox is not null ? box.SecondPin : box.FirstPin;
        }

    FINISH:
        return boxes.Count > 0 ? ([startBox, .. boxes], pin) : ([], pin);

    }
}
