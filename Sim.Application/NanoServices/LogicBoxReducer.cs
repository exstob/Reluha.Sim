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
            .GroupBy(b => new { b.FirstPin, b.SecondPin }) /// group nodeBoxes with same Node
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
            .GroupBy(p => new { p.SecondPin }) /// group nodeBoxes with same Node
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


    public static bool TryBridgeBoxes(List<LogicBox> inputBoxes, out List<LogicBox> outputBoxes)
    {
        bool found = false;
        List<LogicBox> boxes = inputBoxes;
        List<LogicBox> nodeBoxes = inputBoxes.Where(b => b.FirstPin is Node && b.SecondPin is Node).ToList();

        var triangles = new List<List<LogicBox>>();
        // Iterate through all combinations to find the triangle connection
        for (int i = 0; i < nodeBoxes.Count; i++)
        {
            for (int j = i + 1; j < nodeBoxes.Count; j++)
            {
                for (int k = j + 1; k < nodeBoxes.Count; k++)
                {
                    var box1 = nodeBoxes[i];
                    var box2 = nodeBoxes[j];
                    var box3 = nodeBoxes[k];

                    // Check if the three box form a triangle
                    if (IsTriangle(box1, box2, box3))
                    {
                        triangles.Add([box1, box2, box3]);
                    }
                }
            }
        }

        // Compare each pair of triangles in order to find bridge connection 
        for (int i = 0; i < triangles.Count; i++)
        {
            for (int j = i + 1; j < triangles.Count; j++)
            {
                var triangle1 = triangles[i];
                var triangle2 = triangles[j];

                // Find common edge (base)
                var commonBases = triangle1.Intersect(triangle2).ToList();

                if (commonBases.Count == 1) // If exactly one common edge is found
                {
                    var angel1 = triangle1.Except(commonBases).ToList();
                    var angel2 = triangle2.Except(commonBases).ToList();

                    var commonBase = commonBases.Single();
                    var shoulder_1_1 = angel1.First();
                    var shoulder_2_1 = angel2.Find(an => an.Pins().Intersect(shoulder_1_1.Pins()).Count() == 1 );

                    var shoulder_1_2 = angel1[1];
                    var shoulder_2_2 = angel2.Find(an => an.Pins().Intersect(shoulder_1_2.Pins()).Count() == 1);

                    found = true;
                    /// Here we are creating the equivalent schema of bridge connection
                    ///   bridge      __
                    ///     /|\   to  __
                    ///     \|/       ___
                    ///               ___
                    var serialBox1 = new LogicBox([shoulder_1_1, shoulder_2_1]);
                    var serialBox2 = new LogicBox([shoulder_1_2, shoulder_2_2]);
                    var serialBox3 = new LogicBox([shoulder_1_1, commonBase, shoulder_2_2]);
                    var serialBox4 = new LogicBox([shoulder_1_2, commonBase, shoulder_2_1]);

                    var bridgeBox = new LogicBox(LogicBoxType.Parallel)
                    {
                        FirstPin = shoulder_1_1.FirstPin,
                        SecondPin = shoulder_2_1!.SecondPin,
                        Boxes = [serialBox1, serialBox2, serialBox3, serialBox4,],
                    };

                    boxes = boxes.Except([shoulder_1_1!, shoulder_1_2!, shoulder_2_1!, shoulder_2_2!, commonBase!]).ToList();
                    boxes.Add(bridgeBox);

                    var pin1IntersectedWith1 = boxes.Where(b => b.FirstPin.Equals(bridgeBox.FirstPin)).ToList();
                    var pin1IntersectedWith2 = boxes.Where(b => b.SecondPin.Equals(bridgeBox.FirstPin)).ToList();
                    if ((pin1IntersectedWith1.Count + pin1IntersectedWith2.Count) < 3 && bridgeBox.FirstPin is Node node1) /// it means we can simplified the node
                    {
                        var pin = node1.RelayPin ?? new Pin(node1.Id);
                        pin1IntersectedWith1.ForEach(box => box.FirstPin = pin);
                        pin1IntersectedWith2.ForEach(box => box.SecondPin = pin);
                    }


                    var pin2IntersectedWith1 = boxes.Where(b => b.FirstPin.Equals(bridgeBox.SecondPin)).ToList();
                    var pin2IntersectedWith2 = boxes.Where(b => b.SecondPin.Equals(bridgeBox.SecondPin)).ToList();
                    if ((pin2IntersectedWith1.Count + pin2IntersectedWith2.Count) < 3 && bridgeBox.SecondPin is Node node2) /// it means we can simplified the node
                    {
                        var pin = node2.RelayPin ?? new Pin(node2.Id);
                        pin2IntersectedWith1.ForEach(box => box.FirstPin = pin);
                        pin2IntersectedWith2.ForEach(box => box.SecondPin = pin);
                    }
                }
            }
        }

        outputBoxes = boxes;
        return found;
    }

    static private bool IsTriangle(LogicBox b1, LogicBox b2, LogicBox b3)
    {
        var ends = new HashSet<ILogicEdge> { b1.FirstPin, b1.SecondPin, b2.FirstPin, b2.SecondPin, b3.FirstPin, b3.SecondPin };
        return ends.Count == 3;
    }

    static private (List<LogicBox> serialBoxes, ILogicEdge lastPin) TryFindSerialBoxes(LogicBox startBox, List<LogicBox> inputBoxes)
    {
        ILogicEdge pin = startBox.SecondPin;
        var box = startBox;

        List<LogicBox> boxes = [];

        while (true)
        {
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
