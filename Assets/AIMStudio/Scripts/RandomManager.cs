using Elympics;

namespace AIMStudio.Scripts
{
    public class RandomManager : ElympicsMonoBehaviour
    {
        public ElympicsInt randomSeed = new ElympicsInt();
        public System.Random InitializedRandom { get; private set; }

        public void InitializeRandom()
        {
            //We use system random with set seed to make sure, that while random, the stages we will be spawning will be the same on both server and client
            InitializedRandom = new System.Random(randomSeed.Value);
        }

        public void SetSeed(int seed)
        {
            randomSeed.Value = seed;
        }
    }
}