
using Genesis.DAL.Contract.Dtos;

namespace Genesis.DAL.Contract.Repositories
{
    public interface IPicturesRepository
    {
        void Add(PictureDto picture);

        void Add(IEnumerable<PictureDto> pictures);

        void Delete(string publicId);

        void DeleteByPersonId(int personId);

        void DeleteByTreeId(int treeId);

        void Delete(IEnumerable<int> pictureIds);

        PictureDto Get(int pictureId);

        IEnumerable<PictureDto> GetPersonPictures(int personId, bool trackEntities = false);

        Task<PictureDto> GetPersonAvatarAsync(int personId, bool trackEntity = false);

        void ChangeUserAvatar(int personId, int pictureId);
    }
}
