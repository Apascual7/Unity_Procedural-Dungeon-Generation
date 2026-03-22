using UnityEditor;
using UnityEngine;

namespace ProceduralDungeonGeneration
{
    [CustomEditor(typeof(AbstractDungeonGenerator), true)]
    public class RandomDungeonGeneratorEditor : Editor
    {
        AbstractDungeonGenerator _generator;

        private void Awake()
        {
            _generator = (AbstractDungeonGenerator)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Create Dungeon"))
            {
                _generator.GenerateDungeon();
            }
        }
    }
}
