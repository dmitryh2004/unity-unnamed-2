using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdaptiveDifficultyManager : MonoBehaviour
{
    [Range(-1, 5)]
    [SerializeField] int forgettingDegree = 0;

    [Range(-1, 5)]
    [SerializeField] int alertnessDegree = 0;

    [SerializeField] AdaptiveDifficultyValues values;
    [SerializeField] AlertnessUIController uiController;
    [SerializeField] LevelGenerator levelGenerator;
    public static AdaptiveDifficultyManager Instance = null;

    [SerializeField] bool useRoomWeights = false;
    [SerializeField] List<AD_RoomWeight> roomWeights = new();

    [Header("Constants")]
    [SerializeField] float attenuationCoeff = 0.25f; // L - коэффициент затухания
    [SerializeField] float forgettingCoeff = 3f; // k - коэффициент забывания
    [SerializeField] float remainingCoeff = 0.5f; // w'/w - минимальное соотношение нового и старого весов

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (levelGenerator == null) levelGenerator = FindFirstObjectByType<LevelGenerator>();
        uiController?.UpdateUI(AlertnessDegree);
    }

    public AdaptiveDifficultyValues Values => values;
    public int ForgettingDegree => forgettingDegree;
    public int AlertnessDegree => alertnessDegree;
    public void SetForgettingDegree(int forgettingDegree) => this.forgettingDegree = forgettingDegree;
    public void SetAlertnessDegree(int alertnessDegree)
    {
        this.alertnessDegree = alertnessDegree;
        uiController?.UpdateUI(AlertnessDegree);
    }
    public bool UseRoomWeights => useRoomWeights;
    public List<AD_RoomWeight> RoomWeights => roomWeights;
    public void SetRoomWeights(List<AD_RoomWeight> roomWeights) => this.roomWeights = roomWeights;
    public string LocationName => SceneManager.GetActiveScene().name;
    public void ApplyRoomWeights(List<RoomObject> roomsToApply)
    {
        if (!UseRoomWeights) return; // если не используем веса комнат, то выход
        if (roomWeights.Count == 0) return; // если никаких весов комнат нет, то выход

        Dictionary<RoomObject, bool> roomApplyStatuses = new (); // создаем словарь для хранения информации о том, какие комнаты мы нашли в сохранении
        foreach (var room in roomsToApply)
        {
            roomApplyStatuses.Add(room, false);
        }

        foreach (var item in roomWeights) // пытаемся применить веса из сохранения к сгенерированным комнатам
        {
            RoomObject roomObject = levelGenerator.GetRoomByPosition(item.roomPosition, 2.5f);
            if (roomObject != null)
            {
                roomObject.SetRoomWeight(item.weight);
                roomApplyStatuses[roomObject] = true;
            }
        }

        foreach (var room in roomsToApply) // рассчитываем веса для комнат, которых нет в списке
        {
            if (roomApplyStatuses[room]) continue; // если вес уже рассчитан, пропускаем
            Dictionary<RoomObject, int> nearestWeightedRooms = new();
            Queue<KeyValuePair<RoomObject, int>> nearestRooms = new ();
            Dictionary<RoomObject, int> checkedRooms = new ();
            nearestRooms.Enqueue(new KeyValuePair<RoomObject, int>(room, 0));

            while (nearestRooms.Count > 0) // шаг 1. ищем комнаты с уже расставленными весами в ширину
            {
                KeyValuePair<RoomObject, int> currentRoom = nearestRooms.Dequeue();
                //checkedRooms.Add(currentRoom.Key);
                checkedRooms[currentRoom.Key] = currentRoom.Value;
                for (int i = 0; i < currentRoom.Key.GetMaxNeighboursCount(); i++) // перебираем соседей
                {
                    RoomObject neighbour = currentRoom.Key.GetNeighbour(i);
                    if (neighbour == null) continue; // если null, пропускаем
                    if (checkedRooms.ContainsKey(neighbour)) continue; // уже проверяли, пропускаем

                    int distance = currentRoom.Value + 1; // устанавливаем дистанцию до соседа равной дистанции до комнаты + 1
                    for (int j = 0; j < neighbour.GetMaxNeighboursCount(); j++) // проверяем соседей соседа
                    {
                        RoomObject neighbour2 = neighbour.GetNeighbour(j);
                        if (neighbour2 == null) continue;
                        if (checkedRooms.ContainsKey(neighbour2)) // если соседа2 уже проверяли
                        {
                            distance = Mathf.Min(distance, checkedRooms[neighbour2]); // обновляем дистанцию до текущей комнаты
                        }
                    }

                    if (roomApplyStatuses.ContainsKey(neighbour) && roomApplyStatuses[neighbour]) // уже есть вес, записываем и пропускаем
                    {
                        if (!nearestWeightedRooms.ContainsKey(neighbour))
                            nearestWeightedRooms.Add(neighbour, distance);
                        continue;
                    }
                    nearestRooms.Enqueue(new KeyValuePair<RoomObject, int>(neighbour, distance)); // записываем соседа в очередь для перебора
                }
            }

            // если комнаты не найдены, пропускаем
            if (nearestWeightedRooms.Count == 0)
            {
                roomApplyStatuses[room] = true;
                continue;
            }

            // шаг 2. рассчитываем вес для комнаты
            int minDistance = nearestWeightedRooms.Values.ToList().Min();
            float weightSum = 0f;
            foreach (var weightedRoom in nearestWeightedRooms)
            {
                float k = 1 / (weightedRoom.Value - minDistance + 1);
                weightSum += k * weightedRoom.Key.RoomWeight;
            }

            weightSum /= nearestWeightedRooms.Count;
            weightSum -= attenuationCoeff * minDistance;

            // шаг 3. присваиваем вес комнате и помечаем ее как взвешенную
            room.SetRoomWeight(weightSum);
            roomApplyStatuses[room] = true;
        }
    }

    public void UpdateRoomWeights(List<RoomObject> roomsToUpdate)
    {
        if (!UseRoomWeights) return;

        foreach (RoomObject room in roomsToUpdate)
        {
            AD_RoomWeight roomWeightEntry = roomWeights.Find((x) => x.roomPosition == room.GetCenter());
            if (roomWeightEntry != null)
            {
                roomWeightEntry.weight = GetRecalculatedWeight(room.RoomWeight, room.Activity);
            }
            else
            {
                AD_RoomWeight roomWeight = new ();
                roomWeight.roomPosition = room.GetCenter();
                roomWeight.weight = GetRecalculatedWeight(room.RoomWeight, room.Activity);
                roomWeights.Add(roomWeight);
            }
        }
    }

    float GetRecalculatedWeight(float weight, float activity)
    {
        if (activity > 0) return weight + activity;
        else
        {
            float newWeight = weight - forgettingCoeff;
            if (newWeight < 0) newWeight = 0;
            if (newWeight / weight < remainingCoeff) newWeight = weight * remainingCoeff;
            return newWeight;
        }
    }
}
