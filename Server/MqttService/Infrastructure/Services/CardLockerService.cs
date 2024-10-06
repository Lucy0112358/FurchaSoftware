using Domain.Entities;
using Domain.Enums;
using MqttService.Application.Models.MqttRequest;
using MqttService.Application.Repositories;

namespace MqttService.Infrastructure.Services
{
    public class CardLockerService
    {
        private readonly CardLockerRepository cardLockerRepository;
        public CardLockerService(CardLockerRepository cardLockerRepository)
        {
            this.cardLockerRepository = cardLockerRepository;
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
