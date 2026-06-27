namespace FurchaBLL.MqttModels.Subscribe
{
    public class MqttBaseRequest<T>
    {
        public int Command { get; set;  }

        public int Action { get; set; } // 1, 2, 3

        public int Operation { get; set;  }
        public long? TaskId { get; set;  }

        public T Data { get; set;  }

        public DateTime ReceivedDate { get; set; }

        public string? ErrorMessage { get; set; } = null;
    }
}
