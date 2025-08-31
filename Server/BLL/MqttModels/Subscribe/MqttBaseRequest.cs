namespace FurchaBLL.MqttModels.Subscribe
{
    public class MqttBaseRequest<T>
    {
        public int Command { get; set;  }

        public int Operation { get; set;  }

        public T Data { get; set;  }

        public DateTime ReceivedDate { get; set; }

        public string? ErrorMessage { get; set; } = null;
    }
}
