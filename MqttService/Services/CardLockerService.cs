using MqttService.Enums;
using MqttService.Models.MqttRequest;
using MqttService.Repositories;

namespace MqttService.Services
{
    public class CardLockerService
    {
        private readonly CardLockerRepository cardLockerRepository;
        public CardLockerService(CardLockerRepository cardLockerRepository)
        {
            this.cardLockerRepository = cardLockerRepository;
        }

        /// <summary>
        /// Checks all the cases if the locker can be open based on the provided id values
        /// </summary>
        public MqttErrorCodeEnum OpenLocker(MqttRequest request)
        {
            if (request == null || request.lockerId == null || request.cardId == null)
            {
                return MqttErrorCodeEnum.GenericErrorRetry;
            }

            var isCardAndLockerMatching = cardLockerRepository.CanCardOpenLocker(request.cardId, request.lockerId);

            if (!isCardAndLockerMatching)
            {
                return MqttErrorCodeEnum.InvalidCard;
            }

            return MqttErrorCodeEnum.Success;
        }
    }
}
