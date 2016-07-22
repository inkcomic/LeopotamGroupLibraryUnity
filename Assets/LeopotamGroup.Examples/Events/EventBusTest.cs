using LeopotamGroup.Events;
using UnityEngine;

namespace LeopotamGroup.Examples.EventsTest {
    class TestEvent1 {
        public int IntValue;

        public string StringValue;

        public override string ToString () {
            return string.Format ("[IntValue: {0}, StringValue: {1}]", IntValue, StringValue);
        }
    }

    class TestEvent2 {
        public float FloatValue;

        public object ObjectValue;

        public override string ToString () {
            return string.Format ("[FloatValue: {0}, ObjectValue: {1}]", FloatValue, ObjectValue);
        }
    }

    class EventBusTest : MonoBehaviour {
        EventBus _bus;

        void Awake () {
            _bus = new EventBus ();
        }

        void OnEnable () {
            _bus.Subscribe<TestEvent1> (OnEvent1);
            _bus.Subscribe<TestEvent1> (OnEvent2);
            _bus.Subscribe<TestEvent1> (d => Debug.Log ("[EVENT1-SUBSCRIBER3] => " + d));

            _bus.Subscribe<TestEvent2> (OnEvent3);

            var data1 = new TestEvent1
            {
                IntValue = 1,
                StringValue = "123"
            };

            var data2 = new TestEvent2
            {
                FloatValue = 123.456f,
                ObjectValue = "String as object"
            };

            _bus.Publish (data1);

            _bus.Publish (data2);
        }

        void OnDisable () {
            _bus.UnsubscribeAll<TestEvent1> ();
            _bus.UnsubscribeAll<TestEvent2> ();
        }

        void OnEvent1 (TestEvent1 msg) {
            Debug.Log ("[EVENT1-SUBSCRIBER1] => " + msg);
            EventBus bus1 = null;
            bus1.Publish<object> (null);
        }

        void OnEvent2 (TestEvent1 msg) {
            Debug.Log ("[EVENT1-SUBSCRIBER2] => " + msg);
        }

        void OnEvent3 (TestEvent2 msg) {
            Debug.Log ("[EVENT2-SUBSCRIBER] => " + msg);
        }
    }
}