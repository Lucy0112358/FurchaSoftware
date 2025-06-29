namespace FurchaBLL.MqttModels.Subscribe
{
    public class MqttBaseRequest<T>
    {
        public int Command { get; set;  }

        public int Operation { get; set;  }

        public List<T> Data { get; set;  }

        public DateTime ReceivedDate { get; set; }
    }
}
