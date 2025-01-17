using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Alteruna.Trinity;
using TMPro;
using UnityEngine.Events;

namespace Alteruna
{
	public class RoomMenu : CommunicationBridge
	{
		[SerializeField] private TextMeshPro TitleText;
        [SerializeField] private GameObject LANEntryPrefab;
        [SerializeField] private GameObject sessionPrefab;
        [SerializeField] private Transform ContentContainer;
        [SerializeField] private GameObject StartButtonGameObject; // Changed to GameObject
        [SerializeField] private GameObject LeaveButtonGameObject; // Changed to GameObject

        public bool ShowUserCount = false;
        public bool AutomaticallyRefresh = true;
        public float RefreshInterval = 5.0f;
		Room room;
        private readonly List<RoomObject> _roomObjects = new List<RoomObject>();
        private float _refreshTime;

        private int _count;
        private string _connectionMessage = "Connecting";
        private float _statusTextTime;
        private int _roomI = -1;

        private CustomSpatialUIButton StartButton; // Button reference
        private CustomSpatialUIButton LeaveButton; // Button reference

		private Multiplayer mult;

        private void Start()
        {            // Get Button component from GameObject
            StartButton = StartButtonGameObject.GetComponent<CustomSpatialUIButton>();
            LeaveButton = LeaveButtonGameObject.GetComponent<CustomSpatialUIButton>();

            if (Multiplayer == null)
            {
                Multiplayer = FindObjectOfType<Multiplayer>();
            }

            if (Multiplayer == null)
            {
                Debug.LogError("Unable to find a active object of type Multiplayer.");
                if (TitleText != null) TitleText.text = "Missing Multiplayer Component";
                enabled = false;
            }
            else
            {	mult= FindObjectOfType<Multiplayer>();
                Multiplayer.OnConnected.AddListener(Connected);
                Multiplayer.OnDisconnected.AddListener(Disconnected);
                Multiplayer.OnRoomListUpdated.AddListener(UpdateList);
                Multiplayer.OnRoomJoined.AddListener(JoinedRoom);
                Multiplayer.OnRoomLeft.AddListener(LeftRoom);

			// 	StartButton.onClick.AddListener(() =>
            //     {
			// 					Debug.Log("Start Button clicked");
            //         Multiplayer.JoinOnDemandRoom();
            //         _refreshTime = RefreshInterval;
            //     });

            //     LeaveButton.onClick.AddListener(() =>
            //     {
            //         Multiplayer.CurrentRoom?.Leave();
            //         _refreshTime = RefreshInterval;
            //     });
            }

            // Adjust interactable property through Button component
            // StartButton.interactable = true;
            // LeaveButton.interactable = true;
        }


		public void OnStartClicked()
		{
			Debug.Log("Start Button clicked");
			Multiplayer.JoinOnDemandRoom();
            _refreshTime = RefreshInterval;
		}

		public void OnLeaveClicked()
		{
			Debug.Log("Leave Button clicked");
            Multiplayer.CurrentRoom?.Leave();
            _refreshTime = RefreshInterval;
		}


		private void FixedUpdate()
		{
			if (!Multiplayer.enabled)
			{
				TitleText.text = "Offline";
			}
			else if (Multiplayer.IsConnected)
			{
				if (!AutomaticallyRefresh || (_refreshTime += Time.fixedDeltaTime) < RefreshInterval) return;
				_refreshTime -= RefreshInterval;

				Multiplayer.RefreshRoomList();

				if (TitleText == null) return;

				ResponseCode blockedReason = Multiplayer.GetLastBlockResponse();

				if (blockedReason == ResponseCode.NaN) return;

				string str = blockedReason.ToString();
				str = string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x : x.ToString()));
				TitleText.text = str;
			}
			else if ((_statusTextTime += Time.fixedDeltaTime) >= 1)
			{
				_statusTextTime -= 1;
				ResponseCode blockedReason = Multiplayer.GetLastBlockResponse();
				if (blockedReason != ResponseCode.NaN)
				{
					string str = blockedReason.ToString();
					str = string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x : x.ToString()));
					TitleText.text = str;
					return;
				}

				switch (_count)
				{
					case 0:
						TitleText.text = _connectionMessage + ".  ";
						break;
					case 1:
						TitleText.text = _connectionMessage + ".. ";
						break;
					default:
						TitleText.text = _connectionMessage + "...";
						_count = -1;
						break;
				}

				_count++;
			}
		}

		public bool JoinRoom(string roomName, ushort password = 0)
		{
			roomName = roomName.ToLower();
			if (Multiplayer != null && Multiplayer.IsConnected)
			{
				foreach (var room in Multiplayer.AvailableRooms)
				{
					if (room.Name.ToLower() == roomName)
					{
						room.Join(password);
						return true;
					}
				}
			}

			return false;
		}

		private void Connected(Multiplayer multiplayer, Endpoint endpoint)
		{
			// if already connected to room
			if (multiplayer.InRoom)
			{
				JoinedRoom(multiplayer, multiplayer.CurrentRoom, multiplayer.Me);
				return;
			}

			// StartButton.interactable = false;
			// LeaveButton.interactable = false;

			if (TitleText != null)
			{
				TitleText.text = "Rooms";
			}
		}

		private void Disconnected(Multiplayer multiplayer, Endpoint endPoint)
		{
			// StartButton.interactable = false;
			// LeaveButton.interactable = false;

			_connectionMessage = "Reconnecting";
			if (TitleText != null)
			{
				TitleText.text = "Reconnecting";
			}
		}

		private void JoinedRoom(Multiplayer multiplayer, Room room, User user)
		{
			// StartButton.interactable = false;
			// LeaveButton.interactable = true;

			if (TitleText != null)
			{
				TitleText.text = "In Room " + room.Name;
			}
		}

		private void LeftRoom(Multiplayer multiplayer)
		{
			_roomI = -1;

			// StartButton.interactable = true;
			// LeaveButton.interactable = false;

			if (TitleText != null)
			{
				TitleText.text = "Rooms";
			}
		}

		private void UpdateList(Multiplayer multiplayer)
		{
			if (ContentContainer == null) return;

			RemoveExtraRooms(multiplayer);

			for (int i = 0; i < multiplayer.AvailableRooms.Count; i++)
			{
				room = multiplayer.AvailableRooms[i];
				RoomObject entry;

				if (_roomObjects.Count > i)
				{
					if (room.Local != _roomObjects[i].Lan)
					{
						Destroy(_roomObjects[i].GameObject);
						GameObject inst = Instantiate(sessionPrefab, ContentContainer);
						//inst.SetActive(true);

						entry = new RoomObject(inst, room.ID, room.Local);
						_roomObjects[i] = entry;
						CustomSpatialUIButton joinButton = inst.GetComponentInChildren<CustomSpatialUIButton>();
						joinButton.m_ButtonText = room.Name;
						UnityAction action = () => {
							Debug.LogWarning("JOIN");
							room.Join();
							UpdateList(multiplayer);
						};
						joinButton.pressEvent.AddListener(action);
					}
					else
					{
						entry = _roomObjects[i];
						// entry.Button.onClick.RemoveAllListeners();
					}
				}
				else
				{
						GameObject inst = Instantiate(sessionPrefab, ContentContainer);
						//inst.SetActive(true);

						entry = new RoomObject(inst, room.ID, room.Local);
						CustomSpatialUIButton joinButton = inst.GetComponentInChildren<CustomSpatialUIButton>();
						joinButton.m_ButtonText = room.Name;
						UnityAction action = () => {
							Debug.LogWarning("JOIN");
							room.Join();
							UpdateList(multiplayer);
						};
						joinButton.pressEvent.AddListener(action);	

						double startPositionY = 0.6;
						double spacing = 0.7;

						double posY = startPositionY - spacing * i;
        				inst.transform.localPosition = new Vector3(inst.transform.localPosition.y, (float)posY, inst.transform.localPosition.z);		
						
						_roomObjects.Add(entry);
				}

				string newName = room.Name;
				if (ShowUserCount)
				{
					newName += " (" + room.GetUserCount() + "/" + room.MaxUsers + ")";
				}

				if (entry.GameObject.name != newName)
				{
					entry.GameObject.name = newName;
					//entry.Text.text = newName;
				}

				entry.GameObject.SetActive(true);
			}
		}

		public void OnJoinClicked()
		{
			room.Join();
			UpdateList(mult);
		}

		private void RemoveExtraRooms(Multiplayer multiplayer)
		{
			int l = _roomObjects.Count;
			if (multiplayer.AvailableRooms.Count < l)
			{
				for (int i = 0; i < l; i++)
				{
					if (multiplayer.AvailableRooms.All(t => t.ID != _roomObjects[i].ID))
					{
						Destroy(_roomObjects[i].GameObject);
						_roomObjects.RemoveAt(i);
						i--;
						l--;
						if (multiplayer.AvailableRooms.Count >= l) return;
					}
				}
			}
		}

		private struct RoomObject
		{
			public readonly GameObject GameObject;
			public readonly TextMeshPro Text;
			public readonly Button Button;
			public readonly uint ID;
			public readonly bool Lan;

			public RoomObject(GameObject obj, uint id, bool lan = false)
			{
				GameObject = obj;
				Text = obj.GetComponentInChildren<TextMeshPro>();
				Button = obj.GetComponentInChildren<Button>();
				ID = id;
				Lan = lan;
			}
		}
	}
}