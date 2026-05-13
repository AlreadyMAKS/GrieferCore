using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace GrieferCore.Scripts
{
    public static class WorldAuthSystem
    {
        private class WorldAuthData
        {
            public string WorldId;
            public Dictionary<ulong, List<ulong>> Trusted = new();
        }

        private static WorldAuthData _data;
        private static string _path;

        // ---------------- INIT ----------------

        public static void Init(string worldId)
        {
            if (string.IsNullOrEmpty(worldId))
                worldId = "unknown_world";
            
            var dir = Path.Combine(BepInEx.Paths.ConfigPath, "GrieferCore");
            Directory.CreateDirectory(dir);

            _path = Path.Combine(dir, $"auth_{worldId}.json");

            Load();
        }

        // ---------------- LOAD / SAVE ----------------

        private static void Load()
        {
            try
            {
                if (File.Exists(_path))
                {
                    _data = JsonConvert.DeserializeObject<WorldAuthData>(
                        File.ReadAllText(_path)
                    );
                }

                if (_data == null)
                {
                    _data = new WorldAuthData();
                }

                if (_data.Trusted == null)
                {
                    _data.Trusted = new Dictionary<ulong, List<ulong>>();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[WorldAuthSystem] Load error: " + e);

                _data = new WorldAuthData
                {
                    Trusted = new Dictionary<ulong, List<ulong>>()
                };
            }
        }

        public static void Save()
        {
            try
            {
                File.WriteAllText(_path,
                    JsonConvert.SerializeObject(_data, Formatting.Indented));
            }
            catch (Exception e)
            {
                Debug.LogError("[WorldAuthSystem] Save error: " + e);
            }
        }

        // ---------------- API ----------------

        public static void Authorize(ulong ownerId, ulong targetId)
        {
            if (!_data.Trusted.ContainsKey(ownerId))
                _data.Trusted[ownerId] = new List<ulong>();

            if (!_data.Trusted[ownerId].Contains(targetId))
                _data.Trusted[ownerId].Add(targetId);

            Save();
        }

        public static void Remove(ulong ownerId, ulong targetId)
        {
            if (!_data.Trusted.ContainsKey(ownerId))
                return;

            _data.Trusted[ownerId].Remove(targetId);

            Save();
        }

        public static bool CanInteract(ulong ownerId, ulong playerId)
        {
            if (_data == null)
                return false;

            if (_data.Trusted == null)
                return false;

            if (ownerId == playerId)
                return true;

            return _data.Trusted.ContainsKey(ownerId) &&
                   _data.Trusted[ownerId].Contains(playerId);
        }

        // ---------------- DEBUG ----------------

        public static void Print()
        {
            Debug.Log(JsonConvert.SerializeObject(_data, Formatting.Indented));
        }
    }
}