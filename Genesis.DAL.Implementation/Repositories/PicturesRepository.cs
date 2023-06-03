using Genesis.Common.Exceptions;
using Genesis.Common.Extensions;
using Genesis.DAL.Contract.Dtos;
using Genesis.DAL.Contract.Repositories;
using Genesis.DAL.Implementation.Context;
using Microsoft.EntityFrameworkCore;

namespace Genesis.DAL.Implementation.Repositories
{
    public class PicturesRepository : RepositoryBase<PictureDto>, IPicturesRepository
    {
        public PicturesRepository(GenesisDbContext dbContext) : base(dbContext)
        {
        }

        public void Add(PictureDto picture)
        {
            DbContext.Pictures.Add(picture);
        }

        public void Add(IEnumerable<PictureDto> pictures)
        {
            DbContext.Pictures.AddRange(pictures);
        }

        public void Delete(string publicId)
        {
            var pic = DbContext.Pictures.FirstOrDefault(p => p.PublicId == publicId);

            DbContext.Pictures.Remove(pic);
        }

        public void Delete(IEnumerable<int> pictureIds)
        {
            var pics = DbContext.Pictures.Where(p => pictureIds.Contains(p.Id));

            DbContext.Pictures.RemoveRange(pics);
        }

        public PictureDto Get(int pictureId)
        {
            if (!DbContext.Pictures.TryGetSingleValue(p => p.Id == pictureId, out PictureDto pic))
            {
                throw new GenesisDalException("Image doesn't exist", new object[] { pictureId });
            }

            return pic;
        }

        public Task<PictureDto> GetPersonAvatarAsync(int personId, bool trackEntity = false)
        {
            IQueryable<PictureDto> model = DbContext.Pictures;

            if (!trackEntity)
                model = model.AsNoTracking();

            return model.FirstOrDefaultAsync(p => p.PersonId == personId && p.IsMain);
        }

        public IEnumerable<PictureDto> GetPersonPictures(int personId, bool trackEntities = false)
        {
            IQueryable<PictureDto> model = DbContext.Pictures;

            if (!trackEntities)
                model = model.AsNoTracking();

            return model.Where(p => p.PersonId == personId);
        }

        public void ChangeUserAvatar(int personId, int pictureId)
        {
            if (!DbContext.Persons.Include(p => p.Photos).TryGetSingleValue(p => p.Id == personId, out PersonDto person))
            {
                throw new GenesisDalException("Person doesn't exist", new object[] { personId });
            }

            var pictures = person.Photos.ToList();

            if (pictures.TryGetValue(p => p.IsMain, out PictureDto oldAvatar))
            {
                oldAvatar.IsMain = false;
            }

            if (pictures.TryGetValue(p => p.Id == pictureId, out PictureDto newAvatar))
            {
                newAvatar.IsMain = true;
            }
        }

        public void DeleteByPersonId(int personId)
        {
            DbContext.Pictures.RemoveRange(DbContext.Pictures.Where(pic => pic.PersonId == personId));
        }

        public void DeleteByTreeId(int treeId)
        {
            if (DbContext.Pictures.TryGetSingleValue(pic => pic.GenealogicalTreeId == treeId, out PictureDto picture))
            {
                DbContext.Pictures.Remove(picture);
            }
        }
    }
}
