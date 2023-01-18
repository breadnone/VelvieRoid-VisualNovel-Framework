using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace VelvieR
{
    public static class GameObjectManager
    {
        public static List<(GameObject obj, string name, int id, string customTag)> gameObjectManager = new List<(GameObject, string, int, string)>();
        public static void AddToGameObjectManager((GameObject obj, string name, int id, string customTag) prop)
        {
            if (!gameObjectManager.Exists(x => x.id == prop.id))
            {
                gameObjectManager.Add(prop);
            }
        }
        public static void RemoveFromGameObjectManager((GameObject obj, string name, int id, string customTag) prop)
        {
            if (gameObjectManager.Exists(x => x.id == prop.id))
            {
                gameObjectManager.Remove(prop);
            }
        }
        public static void SetPositionFromGameObjectManager(string oname, Vector3 position, bool isLocal = false)
        {
            for (int i = 0; i < gameObjectManager.Count; i++)
            {
                if (gameObjectManager[i].name.Contains(oname))
                {
                    if (!isLocal)
                        gameObjectManager[i].obj.transform.position = position;
                    else
                        gameObjectManager[i].obj.transform.localPosition = position;
                }
            }
        }
        public static List<GameObject> GetGameObjectFromGameObjectManager(string oname, string idFromFind = "")
        {
            List<GameObject> gos = new List<GameObject>();

            if(!String.IsNullOrEmpty(idFromFind))
            {
                for (int i = 0; i < gameObjectManager.Count; i++)
                {
                    if (gameObjectManager[i].name.Contains(oname))
                    {
                        gos.Add(gameObjectManager[i].obj);
                    }
                }
            }
            else
            {
                for (int i = 0; i < gameObjectManager.Count; i++)
                {
                    if (gameObjectManager[i].name.Contains(idFromFind))
                    {
                        gos.Add(gameObjectManager[i].obj);
                    }
                }
            }

            return gos;
        }
    }
}