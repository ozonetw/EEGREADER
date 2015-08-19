using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emotiv;

namespace ExperimentalEEG
{
    class Program
    {
        static EmoEngine engine = EmoEngine.Instance;
        static EmoState eState;
        static uint userId = 0;
        static int xmax= 0, ymax = 0;
        static bool allow = false;
        static float elapsed;

        static void Main(string[] args)
        {
            engine.EmoEngineEmoStateUpdated += new EmoEngine.EmoEngineEmoStateUpdatedEventHandler(engine_EmoStateUpdated);
            engine.Connect();

            while (true)
            {
                engine.ProcessEvents(1000);
                if (allow)
                {
                    int x = 0, y = 0;
                    engine.HeadsetGetGyroDelta(userId, out x, out y);
                    xmax += x;
                    ymax += y;
                    Console.WriteLine("xmax: {0}, ymax : {1}", xmax, ymax);
                    
                }
                else
                {
                    Console.WriteLine("Elapsed :  {0}, Engagement: {1}, Frustration: {2}, Medidation: {3}, Signal: {4}", elapsed,
                                                                                               (eState != null ? eState.AffectivGetEngagementBoredomScore().ToString(): "null"),
                                                                                               (eState != null ? eState.AffectivGetFrustrationScore().ToString(): "null"),
                                                                                               (eState != null ? eState.AffectivGetMeditationScore().ToString() : "null"),
                                                                                               (eState != null ? eState.GetWirelessSignalStatus().ToString() : "null"));
                }
            }
        }

        static void engine_EmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {
            eState = e.emoState;
            elapsed = eState.GetTimeFromStart();

            if ((elapsed > 5) && (!allow))
            {
                allow = true;
            }

            if (eState.GetWirelessSignalStatus() == EdkDll.EE_SignalStrength_t.NO_SIGNAL)
            {
                allow = false;
            }
        }
    }
}
