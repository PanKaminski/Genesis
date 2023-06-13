using AutoMapper;
using Genesis.App.Contract.Common.ApiModels;
using Genesis.App.Contract.Common.Services;
using Genesis.App.Contract.Models;
using Genesis.DAL.Contract.Dtos;
using Genesis.DAL.Contract.UOW;
using Microsoft.AspNetCore.Http;
using System.Collections;

namespace Genesis.App.Implementation.Common.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly ICloudinaryService cloudinaryService;
        private readonly IUnitOfWork unitOfWork;

        public PhotoService(ICloudinaryService cloudinaryService, IUnitOfWork unitOfWork)
        {
            this.cloudinaryService = cloudinaryService;
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PictureResponse>> UploadAsync(IFormFileCollection pictures)
        {
            var images = new List<PictureResponse>();
            await foreach (var picture in UploadImplAsync(pictures))
            {
                images.Add(new PictureResponse
                {
                    Url = picture.Url,
                    PublicId = picture.PublicId,
                });
            }

            return images;
        }

        public async Task SavePersonPictures(int personId, IFormFileCollection picturesModel, bool saveChanges)
        {
            await foreach (var picture in UploadImplAsync(picturesModel))
            {
                picture.PersonId = personId;
                unitOfWork.PicturesRepository.Add(picture);
            }

            if (saveChanges)
                await unitOfWork.CommitAsync();
        }

        public async Task SavePersonPictures(int personId, IEnumerable<Picture> pictures, bool saveChanges)
        {
            unitOfWork.PicturesRepository.Add(pictures.Select(p => new PictureDto
            {
                Url = p.Url,
                PublicId = p.PublicId,
                IsMain = p.IsMain,
                PersonId = personId,
                CreatedTime = DateTime.Now,
            }));

            if (saveChanges)
            {
                await unitOfWork.CommitAsync();
            }
        }

        public void MakeAvatar(int personId, int pictureId, bool saveChanges)
        {
            unitOfWork.PicturesRepository.ChangeUserAvatar(personId, pictureId);

            if (saveChanges)
                unitOfWork.Commit();
        }

        public void RemovePictures(IEnumerable<int> picturesIds, bool saveChanges)
        {
            if (picturesIds is null || !picturesIds.Any()) return;

            unitOfWork.PicturesRepository.Delete(picturesIds);

            if (saveChanges)
                unitOfWork.Commit();
        }

        public IEnumerable<Picture> GetPersonPictures(int personId)
        {
            if (personId <= 0) throw new ArgumentException("Invalid person id", nameof(personId));

            var picturesDtos = unitOfWork.PicturesRepository.GetPersonPictures(personId);

            return picturesDtos.Select(p => new Picture(p.Id, p.Url, p.PublicId, p.IsMain));
        }

        public async Task<Picture> GetPersonAvatarAsync(int personId)
        {
            if (personId <= 0) throw new ArgumentException("Invalid person id", nameof(personId));

            var pictureDto = await unitOfWork.PicturesRepository.GetPersonAvatarAsync(personId);

            return pictureDto is null ? null : new Picture(pictureDto.Id, pictureDto.Url, pictureDto.PublicId, pictureDto.IsMain);
        }

        private async IAsyncEnumerable<PictureDto> UploadImplAsync(IFormFileCollection pictures)
        {
            foreach (var picture in pictures)
            {
                var pictureModel = await this.cloudinaryService.UploadImage(picture);
                var pictureDto = new PictureDto
                {
                    Url = pictureModel.Url,
                    PublicId = pictureModel.PublicId,
                    CreatedTime = pictureModel.CreatedTime,
                };

                yield return pictureDto;
            }
        }

        public void RemovePersonPictures(int personId, bool saveChanges)
        {
            unitOfWork.PicturesRepository.DeleteByPersonId(personId);

            if (saveChanges) unitOfWork.Commit();
        }

        public void RemovePersonsPictures(IEnumerable<int> personsIds, bool saveChanges)
        {
            unitOfWork.PicturesRepository.DeleteByPersonsIds(personsIds);

            if (saveChanges) unitOfWork.Commit();
        }

        public void ChangeCoatOfArms(int treeId, Picture picture, bool saveChanges)
        {
            unitOfWork.PicturesRepository.DeleteByTreeId(treeId);

            unitOfWork.PicturesRepository.Add(new PictureDto
            {
                Url = picture.Url,
                PublicId = picture.PublicId,
                CreatedTime = picture.CreatedTime,
                GenealogicalTreeId = treeId,
            });

            if (saveChanges) unitOfWork.Commit();
        }
    }
}
