using AutoMapper;
using Genesis.App.Contract.Dashboard.Services;
using Genesis.App.Contract.Models;
using Genesis.Common.Enums;
using Genesis.DAL.Contract.Dtos;
using Genesis.DAL.Contract.UOW;

namespace Genesis.App.Implementation.Dashboard.Services
{
    public class RelationsService : IRelationsService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public RelationsService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task AddRelationsAsync(IEnumerable<PersonRelation> relations, int treeId, bool saveChanges = true)
        {
            ArgumentNullException.ThrowIfNull(relations, nameof(relations));

            if (relations.Any())
                await this.unitOfWork.RelationsRepository.AddRelationsAsync(
                    mapper.Map<IEnumerable<PersonRelationDto>>(relations), treeId);

            if (saveChanges) await unitOfWork.CommitAsync();
        }

        public IEnumerable<PersonRelation> GetRelations(int personId, int? treeId = null, Relation? relType = null)
        {
            return mapper.Map<IEnumerable<PersonRelation>>(unitOfWork.RelationsRepository
                .GetRelationsWithPersons(personId, relType, treeId));
        }

        public void RemovePersonRelations(int personId, bool saveChanges)
        {
            unitOfWork.RelationsRepository.RemovePersonRelations(personId);

            if (saveChanges) unitOfWork.Commit();
        }
    }
}
