using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using TMPro;
using System.IO;
using System.Text;

namespace VelvieR
{
    public class VJsonAdapter
    {
        private string json =  string.Empty;
        public string path {get;} = Application.dataPath + "/VelviE-R/SaveData/";

        //TODO
/*        public void StartSerializing(List<VVariableClass> variables, string vobjectGuid)
        {
            json =  string.Empty;
            string fileName = "VelvieR-" + vobjectGuid;
            json = JsonConvert.SerializeObject(variables, Formatting.Indented);
            File.WriteAllText(path + fileName, json);
        }
        */
        public void StartSerializingVPool(VVariablePools variables, string vobjectGuid)
        {
            json =  string.Empty;
            string fileName = "VelvieR-" + vobjectGuid;
            json = JsonConvert.SerializeObject(variables, Formatting.Indented);
            File.WriteAllText(path + fileName, json);
        }
        public VVariablePools StartDeserializingVPool(string vobjectGuid)
        {
            var patFile = path + "VelvieR-" + vobjectGuid;
            VVariablePools tmppool;

            if(File.Exists(patFile))
            {
                string file = File.ReadAllText(patFile);
                tmppool = JsonConvert.DeserializeObject<VVariablePools>(file);
                return tmppool;
            }

            return null;
        }
        public void SaveGame()
        {
            
        }
        public void SaveGOTranformStates()
        {
            
        }
    }
}