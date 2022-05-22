using System.Collections;
using System.Collections.Generic;
using com.cozyhome.Console;
using UnityEngine;

using System.IO;

public class Debug_Inter : MonoBehaviour
{
    [SerializeField] private Transform A, B, Int;

    void Start() {
        MonoConsole.InsertCommand("parse", (string[] modifiers, out string output) => {
            output = "===================";
            if(modifiers == null || string.IsNullOrEmpty(modifiers[0])) {
                MonoConsole.PrintToScreen("error: did not provide filepath");
                return;
            }
            else {
                string fp = Application.dataPath + "/Resources/" + modifiers[0];
                MonoConsole.PrintToScreen($"parsing {fp}");
                Dictionary<int, Intersection> map = new Dictionary<int, Intersection>();
                Simulation Sim = null;

                // begin reading via streamwriter
                using(StreamReader sr = new StreamReader(fp)) {
                    
                    string[] chop = sr.ReadLine().Split();

                    var D = int.Parse(chop[0]); // Duration
                    var I = int.Parse(chop[1]); // # of intersections
                    var S = int.Parse(chop[2]); // # of streets
                    var V = int.Parse(chop[3]); // # of cars
                    var F = int.Parse(chop[4]); // bonus points for each car that reaches dest before D.


                    for(int i = 0; i < S; i++)
                    { // # streets
                        string[] desc = sr.ReadLine().Split();
                        var B = int.Parse(desc[0]);
                        var E = int.Parse(desc[0]);
                        var N = desc[2];
                        var L = int.Parse(desc[3]);
                        Sim   = new Simulation(D, I, S, V, F);

                        // IN will have 
                        var Street = new Street(B, E, N, L);

                        // either lookup or instance
                        // abstraction to save lines here, basically just appends
                        // a new intersection to the database if not already inserted
                        var START = map.ContainsKey(B) ? map[B] : AppendInter(ref map, B);
                        var END   = map.ContainsKey(E) ? map[E] : AppendInter(ref map, B);

                        // notify intersections of edge
                        END.ins.Add(Street);
                    }

                    for (int j = 0; j < V; j++) { // # cars
                        string[] desc = sr.ReadLine().Split();
                        Car c = new Car(int.Parse(desc[0]), desc);
                        Sim.AppendCar(c);
                        
                        // string outp = $"car: {c.P}";
                        // for(int x = 0; x<c.P;x++)
                            // outp += (" " + c.path[x]);
                        // MonoConsole.PrintToScreen(outp);
                    }
                }

                // Simulate(...)
                if(Sim == null) {
                    MonoConsole.PrintToScreen("How tf did we get here?");
                    return;
                }

                

                return;
            }
        });

        MonoConsole.AttemptExecution("parse a.txt ");

        static Intersection AppendInter(ref Dictionary<int, Intersection> map, int B)
        {
            var i = new Intersection();
            map.Add(B, i);
            return i;
        }
    }
    
}

class Simulation {
    public int Duration;
    public int Intersections;
    public int Streets;
    public int NCars;
    public int Bonus;

    public Simulation(int Duration, int Intersections, int Streets, int NCars, int Bonus) {
        this.Intersections = Intersections;
        this.Duration      = Duration;
        this.Streets       = Streets;
        this.Bonus         = Bonus;
        this.NCars         = NCars;

        this.Cars          = new List<Car>();
    }

    public void AppendCar(Car c) => Cars.Add(c);

    public List<Car> Cars;
}

class Intersection {
    public List<Street> ins;

    public Intersection() {
        ins  = new List<Street>();
    }
}

class Street {
    public int    out_inter;
    public int     in_inter;
    public string      name;
    public int         time;

    public Light t_light;

    public Queue<Car> waiting;

    public Street(int in_inter, int out_inter, string name, int time) {
        this.in_inter  = in_inter;
        this.out_inter = out_inter;
        this.name      = name;
        this.time      = time;
        this.t_light   = new Light(1F); // need to assign ourselves
        this.waiting   = new Queue<Car>();
    }

    public void AppendCar(Car c) => waiting.Enqueue(c);
}

class Light {
    private float duration;

    public Light(float duration) {
        this.duration = duration;
    }

    public float Duration => duration;
}

class Car {
    public int P;
    public List<string> path;

    public Car(int P, string[] desc) {
        this.P = P;
        path = new List<string>();
        for(int i = 1;i < desc.Length;i++)
            path.Add(desc[i]);
    }
}