using Business.Abstract;
using Core.Utilities.Business;
using Core.Utilities.Helper;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Business.Concrete
{
    public class CarImageManager : ICarImageService
    {
        ICarImageDal _carImageDal;
        ICarService _carService;

        private readonly string DefaultImage = "default.jpeg";
        public CarImageManager(ICarImageDal carImageDal, ICarService carService)
        {
            _carImageDal = carImageDal;
            _carService = carService;
        }
        public IResult Add(IFormFile image, CarImage img)
        {
            //// IResult result = BusinessRules.Run(CheckIfCarIsExists(img.CarId),
            //                                CheckIfFileExtensionValid(image.FileName),
            //                                CheckIfImageNumberLimitForCar(img.CarId));

            // if (result != null)
            // {
            //     return result;
            // }
            img.ImagePath = FileOperationsHelper.Add(image);
            img.CreatedAt = DateTime.Now;
            _carImageDal.Add(img);
            return new SuccessResult("Image" + "Eklendi");
        }

        public IResult Update(IFormFile image, CarImage img)
        {
            IResult result = BusinessRules.Run(CheckIfImageIsExists(img.Id),
                                           CheckIfFileExtensionValid(image.FileName));
            if (result != null)
            {
                return result;
            }
            var carImg = _carImageDal.Get(x => x.Id == img.Id);
            carImg.CreatedAt = DateTime.Now;
            carImg.ImagePath = FileOperationsHelper.Add(image);
            FileOperationsHelper.Delete(img.ImagePath);
            _carImageDal.Update(carImg);
            return new SuccessResult("Image" + " güncellendi");
        }

        public IResult Delete(CarImage img)
        {
            IResult result = BusinessRules.Run(CheckIfImagePathIsExists(img.ImagePath));
            if (result != null)
            {
                return result;
            }

            _carImageDal.Delete(img);
            FileOperationsHelper.Delete(img.ImagePath);
            return new SuccessResult("Image" + " silindi");
        }

        public IDataResult<CarImage> FindByID(int Id)
        {
            CarImage img = new CarImage();
            if (_carImageDal.GetAll().Any(x => x.Id == Id))
            {
                img = _carImageDal.GetAll().FirstOrDefault(x => x.Id == Id);
            }
            else Console.WriteLine("No such car image was found.");
            return new SuccessDataResult<CarImage>(img);
        }

        public IDataResult<CarImage> Get(CarImage img)
        {
            return new SuccessDataResult<CarImage>(_carImageDal.Get(x => x.Id == img.Id));
        }

        public IDataResult<List<CarImage>> GetAll()
        {
            return new SuccessDataResult<List<CarImage>>(_carImageDal.GetAll());
        }

        public IResult GetList(List<CarImage> list)
        {
            Console.WriteLine("\n------- Car Image List -------");

            foreach (var img in list)
            {
                Console.WriteLine("{0}- Car ID: {1}\n    Image Path: {2}\n    CratedAt: {3}\n", img.Id, img.CarId, img.ImagePath, img.CreatedAt);
            }
            return new SuccessResult();
        }

        public IDataResult<List<CarImage>> GetCarListByCarID(int carID)
        {
            if (!_carImageDal.GetAll().Any(x => x.CarId == carID))
            {
                List<CarImage> img = new List<CarImage>
                {
                    new CarImage
                    {
                        CarId = carID,
                        ImagePath = DefaultImage
                    }
                };
                return new SuccessDataResult<List<CarImage>>(img);
            }
            return new SuccessDataResult<List<CarImage>>(_carImageDal.GetAll(x => x.CarId == carID));
        }

        private IResult CheckIfCarIsExists(int carId)
        {
            if (!_carService.GetAll().Data.Any(x => x.Id == carId))
            {
                return new ErrorResult();
            }
            return new SuccessResult();
        }

        private IResult CheckIfFileExtensionValid(string file)
        {
            if (!Regex.IsMatch(file, @"([A-Za-z0-9\-]+)\.(png|PNG|gif|GIF|jpg|JPG|jpeg|JPEG)"))
            {
                return new ErrorResult();
            }

            return new SuccessResult();
        }

        private IResult CheckIfImagePathIsExists(string path)
        {
            if (!_carImageDal.GetAll().Any(x => x.ImagePath == path))
            {
                return new ErrorResult();
            }

            return new SuccessResult();
        }

        private IResult CheckIfImageNumberLimitForCar(int carId)
        {
            if (_carImageDal.GetAll(x => x.CarId == carId).Count == 5)
            {
                return new ErrorResult();
            }
            return new SuccessResult();
        }

        private IResult CheckIfImageIsExists(int Id)
        {
            if (!_carImageDal.GetAll().Any(x => x.Id == Id))
            {
                return new ErrorResult();
            }
            return new SuccessResult();
        }
    }
}
