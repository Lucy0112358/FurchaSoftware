using Domain.Entities;
using Domain.Enums;
using MqttService.Application.Models.MqttRequest;
using MqttService.Application.Repositories;

namespace MqttService.Application.Services
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

        //Later will handle the empty case
        public List<Locker> GetLockersByCardId(int cardId)
        {
            var lockers = cardLockerRepository.GetLockersByCardId(cardId);

            return lockers;
        }

        public List<Branch> GetAllActiveBranches()
        {
            var allBranches = cardLockerRepository.GetAllActiveBranches();
            return allBranches;
        }

        public List<BrainModule> GetBrainsByBranchId(int branchId)
        {
            var brainsByBranchId = cardLockerRepository.GetBrainsByBranchId(branchId);
            return brainsByBranchId;
        }
    }
}
