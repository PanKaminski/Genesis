using Genesis.App.Contract.Common.ApiModels;
using Genesis.App.Contract.Models;
using Microsoft.AspNetCore.Http;

namespace Genesis.App.Contract.Common.Services
{
    public interface IPhotoService
    {
        Task<IEnumerable<PictureResponse>> UploadAsync(IFormFileCollection pictures);

        Task SavePersonPictures(int personId, IFormFileCollection picturesModel, bool saveChanges);

        Task SavePersonPictures(int personId, IEnumerable<Picture> pictures, bool saveChanges);

        IEnumerable<Picture> GetPersonPictures(int personId);

        Task<Picture> GetPersonAvatarAsync(int personId);

        void RemovePersonPictures(int personId, bool saveChanges);

        void RemovePersonsPictures(IEnumerable<int> personsIds, bool saveChanges);

        void RemovePictures(IEnumerable<int> picturesIds, bool saveChanges);

        void MakeAvatar(int personId, int pictureId, bool saveChanges);

        void ChangeCoatOfArms(int treeId, Picture picture, bool saveChanges);
    }
}
