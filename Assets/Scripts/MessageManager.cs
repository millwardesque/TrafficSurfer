using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Message {
	public MonoBehaviour sender;
	public string name;
	public object data;

	public Message(MonoBehaviour sender, string name, object data) {
		this.sender = sender;
		this.name = name;
		this.data = data;
	}
}

public delegate void MessageReceiver(Message message);

public class MessageManager : MonoBehaviour {
	Dictionary<string, List<MessageReceiver> > listeners = new Dictionary<string, List<MessageReceiver>>();

	public static MessageManager Instance = null;

	void Awake() {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy (gameObject);
		}
	}

	public void AddListener(string messageName, MessageReceiver listener) {
		if (!listeners.ContainsKey(messageName)) {
			listeners[messageName] = new List<MessageReceiver>();
		}
		listeners[messageName].Add(listener);
	}

	public void SendMessage(Message message) {
		if (listeners.ContainsKey(message.name)) {
			List<MessageReceiver> recipients = listeners[message.name];
			for (int i = 0; i < recipients.Count; ++i) {
				recipients[i].Invoke(message);
			}
		}
		else {
			Debug.Log (string.Format("Sending message {0}: No listeners registered.", message.name));
		}
	}
}
