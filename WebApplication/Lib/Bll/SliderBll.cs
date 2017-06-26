using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;
using System.Web;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.Slider;
using WebApplication.Models.User;

namespace WebApplication.Lib.Bll
{
    public class SliderBll
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static ResultModel Create(SliderModel model)
        {
            var resultModel = new ResultModel();
            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {

                        var createdDate = DateTime.Now;
                        var count = 1;
                        var slider = new Slider
                        {
                            Title = model.Title,
                            Description = model.Description,
                            GoToLink = model.GoToLink,
                            Type = model.Type,
                            CreatedDate = DateTime.Now,
                            Status = StatusEnum.Active,
                        };

                        #region [Validation]

                        /*if (model.Title.Trim().Equals(string.Empty))
                        {
                            Log.Info("Title name cannot be empty.");
                            resultModel.setCode(Result.INVALID_DATA);
                            return resultModel;
                        }
                        if (model.Description.Trim().Equals(string.Empty))
                        {
                            Log.Info("Description name cannot be empty.");
                            resultModel.setCode(Result.INVALID_DATA);
                            return resultModel;
                        }*/
                        if (model.GoToLink.Trim().Equals(string.Empty))
                        {
                            Log.Info("GoToLink name cannot be empty.");
                            resultModel.setCode(Result.INVALID_DATA);
                            return resultModel;
                        }
                        if (model.Type.Trim().Equals(string.Empty))
                        {
                            Log.Info("Type name cannot be empty.");
                            resultModel.setCode(Result.INVALID_DATA);
                            return resultModel;
                        }
                        #endregion

                        var sliderActive = dbContext.Sliders.Where(m => m.Type.Equals(model.Type) && m.Status.Equals((int)StatusEnum.Active)).ToList();

                        slider.IOrder = sliderActive.Count + 1;

                        dbContext.Sliders.Add(slider);
                        dbContext.SaveChanges();

                        if (model.Image == null)
                        {
                            Log.Info("Cover cannot be null.");
                            resultModel.setCode(Result.INVALID_DATA);
                            return resultModel;
                        }
                        var coverImg = ImageBll.Save(model.Image, null, string.Format("_{0}_{1}", count++, slider.Id));
                        if (coverImg == null)
                        {
                            Log.Error("Save image fail.");
                            resultModel.setCode(Result.FAIL_COVER);
                            return resultModel;
                        }

                        coverImg.Type = ImageTypeEnum.Cover;
                        coverImg.CreatedDate = createdDate;

                        dbContext.Images.Add(coverImg);
                        dbContext.SaveChanges();

                        slider.ImageId = coverImg.Id;
                        dbContext.SaveChanges();

                        scope.Complete();

                        resultModel.setCode(Result.SUCCESS);

                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("Create Slider", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            return resultModel;
        }

        public static ResultModel getSliderOrBannerByTypeAndStatus(string type, int status)
        {
            var resultModel = new ResultModel();
            try
            {
                using (var dbContext = new Entities())
                {
                    #region [Validation]
                    if (!TypeHomePageEnum.Banner.Equals(type.Trim()) &&
                        !TypeHomePageEnum.Slider.Equals(type.Trim()) && !TypeHomePageEnum.AllSlider.Equals(type.Trim()))
                    {
                        resultModel.setCode(Result.INVALID_DATA);
                        resultModel.Message = resultModel.Message + " [Type]";
                        return resultModel;
                    }
                    #endregion
                    #region [Get HomePage by Type]
                    var sliders = dbContext.Sliders.Where(m => m.Type.Equals(type) && m.Status.Equals(status)).ToList().OrderBy(m => m.IOrder);

                    if (type.Trim().Equals(TypeHomePageEnum.AllSlider))
                    {
                        sliders = dbContext.Sliders.Where(m => m.Status.Equals(status)).ToList().OrderBy(m => m.IOrder);
                    }

                    if (sliders != null)
                    {
                        List<SliderModel> listSliders = new List<SliderModel>();
                        if (sliders.Count() != 0)
                        {
                            foreach (var item in sliders)
                            {
                                SliderModel slider = new SliderModel();

                                slider.Id = item.Id;
                                slider.Title = item.Title;
                                slider.Image_Url = String.Format("/{0}/{1}/{2}", "Image", "View", item.ImageId.ToString());
                                slider.Description = item.Description;
                                slider.GoToLink = item.GoToLink;
                                slider.Status = item.Status;
                                slider.Type = item.Type;

                                listSliders.Add(slider);
                            }
                        }

                        resultModel.setCode(Result.SUCCESS);
                        resultModel.Data = listSliders;
                    }
                    #endregion
                }
            }
            catch (Exception exception)
            {
                Log.Error("getSliderOrBannerByTypeAndStatus", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            return resultModel;
        }

        public static ResultModel UpdateStatusSlider(UserModel currentUser, long sliderId, int status)
        {
            var resultModel = new ResultModel();
            try
            {
                using (var dbContext = new Entities())
                {

                    #region [Validation]
                    if (currentUser == null)
                    {
                        resultModel.setCode(Result.AUTH);
                        return resultModel;
                    }
                    if (status != StatusEnum.InActive && status != StatusEnum.Active)
                    {
                        resultModel.setCode(Result.INVALID_DATA);
                        return resultModel;
                    }
                    #endregion

                    #region [ListAllUser]
                    var accountAdmin = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().Equals(currentUser.Email.Trim())
                                                                && (m.RoleId == RoleEnum.Admin || m.RoleId == RoleEnum.Manager));
                    if (accountAdmin != null)
                    {
                        var slider = dbContext.Sliders.FirstOrDefault(m => m.Id.Equals(sliderId));
                        if (slider != null)
                        {
                            // Update Iorder
                            var listSlider = dbContext.Sliders.Where(m => m.Type.Equals(slider.Type) && m.Status.Equals(StatusEnum.Active)).ToList().OrderBy(m => m.IOrder);
                            if (status.Equals(StatusEnum.InActive))
                            {
                                foreach (var item in listSlider)
                                {
                                    if (listSlider.Count() > slider.IOrder)
                                    {
                                        if (item.IOrder > slider.IOrder)
                                        {
                                            item.IOrder = item.IOrder - 1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                slider.IOrder = listSlider.Count() + 1;
                            }

                            slider.Status = status;
                            dbContext.SaveChanges();

                            resultModel.setCode(Result.SUCCESS);
                            resultModel.Data = slider;
                        }
                        else
                        {
                            resultModel.setCode(Result.FAILED);
                        }
                    }
                    else
                    {
                        resultModel.setCode(Result.ACCOUNT_NOT_ADMIN);
                    }
                    #endregion
                }
            }
            catch (Exception exception)
            {
                Log.Error("Update status Slider", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            return resultModel;
        }

        public static ResultModel IncreateIOrder(UserModel user, long sliderId)
        {
            var resultModel = new ResultModel();
            if (sliderId == 0)
            {
                resultModel.setCode(Result.INVALID_DATA);
                resultModel.Message = resultModel.Message + " [Null ID]";
                return resultModel;
            }

            try
            {
                using (var dbContext = new Entities())
                {
                    using (var dbContextTransaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            #region [Validate]
                            if (user == null || !user.RoleId.Equals((int)RoleEnum.Admin))
                            {
                                resultModel.setCode(Result.AUTH);
                                return resultModel;
                            }
                            #endregion

                            #region [Save HomePage]
                            // Id
                            var slider = dbContext.Sliders.Where(m => m.Id.Equals(sliderId)).FirstOrDefault();
                            if (slider == null)
                            {
                                resultModel.setCode(Result.FAILED);
                            }
                            else
                            {
                                int highIOrder = (int)slider.IOrder - 1;
                                if (highIOrder == 0)
                                {
                                    resultModel.setCode(Result.FAILED);
                                }
                                else
                                {
                                    var sliderHigh = dbContext.Sliders.Where(m => m.Status.Equals(StatusEnum.Active)
                                                                               && m.IOrder.Equals(highIOrder)
                                                                               && m.Type.Trim().Equals(slider.Type.ToString())).FirstOrDefault();
                                    var temp = slider.IOrder;
                                    slider.IOrder = sliderHigh.IOrder;

                                    sliderHigh.IOrder = temp;
                                    dbContext.SaveChanges();

                                    resultModel.Data = slider;
                                    resultModel.setCode(Result.SUCCESS);
                                }
                            }
                            #endregion

                            dbContextTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            dbContextTransaction.Rollback();
                            resultModel.setCode(Result.SYSTEM);
                        }
                    }

                }
            }
            catch (Exception exception)
            {
                Log.Error("Add Or Update HomePage", exception);
                resultModel.setCode(Result.SYSTEM);
                throw exception;
            }

            return resultModel;
        }

        public static ResultModel DecreaseIOrder(UserModel user, long sliderId)
        {
            var resultModel = new ResultModel();
            if (sliderId == 0)
            {
                resultModel.setCode(Result.INVALID_DATA);
                resultModel.Message = resultModel.Message + " [Null ID]";
                return resultModel;
            }
            try
            {
                using (var dbContext = new Entities())
                {
                    using (var dbContextTransaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {
                            #region [Validate]
                            if (user == null || !user.RoleId.Equals((int)RoleEnum.Admin))
                            {
                                resultModel.setCode(Result.AUTH);
                                return resultModel;
                            }
                            #endregion

                            #region [Save HomePage]
                            // Id
                            var slider = dbContext.Sliders.Where(m => m.Id.Equals(sliderId)).FirstOrDefault();
                            if (slider == null)
                            {
                                resultModel.setCode(Result.FAILED);
                            }
                            else
                            {
                                var listHome = dbContext.Sliders.Where(m => m.Type.Equals(slider.Type)
                                                                       && m.Status.Equals(StatusEnum.Active)).ToList<Slider>();
                                int lowOrder = (int)slider.IOrder + 1;
                                if (lowOrder > listHome.Count)
                                {
                                    resultModel.setCode(Result.FAILED);
                                }
                                else
                                {
                                    var homeLow = dbContext.Sliders.Where(m => m.IOrder.Equals(lowOrder)
                                            && m.Type.Equals(slider.Type)
                                            && m.Status.Equals(StatusEnum.Active)).FirstOrDefault();
                                    if (homeLow != null)
                                    {
                                        var temp = slider.IOrder;
                                        slider.IOrder = homeLow.IOrder;

                                        homeLow.IOrder = temp;
                                        dbContext.SaveChanges();

                                        resultModel.Data = slider;
                                        resultModel.setCode(Result.SUCCESS);
                                    }
                                    else
                                    {
                                        resultModel.setCode(Result.FAILED);
                                    }
                                }
                            }

                            #endregion

                            dbContextTransaction.Commit();
                        }
                        catch (Exception)
                        {
                            dbContextTransaction.Rollback();
                            resultModel.setCode(Result.SYSTEM);
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("Add Or Update HomePage", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            return resultModel;
        }

        public static ResultModel UpdateSlider(UserModel currentUser, SliderModel model)
        {
            var resultModel = new ResultModel();
            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        var createdDate = DateTime.Now;
                        var count = 1;

                        #region [Validation]

                        /*if (model.Title.Trim().Equals(string.Empty))
                        {
                            Log.Info("Title name cannot be empty.");
                            resultModel.setCode(Result.INVALID_DATA);
                            return resultModel;
                        }
                        if (model.Description.Trim().Equals(string.Empty))
                        {
                            Log.Info("Description name cannot be empty.");
                            resultModel.setCode(Result.INVALID_DATA);
                            return resultModel;
                        }*/
                        if (model.GoToLink.Trim().Equals(string.Empty))
                        {
                            Log.Info("GoToLink name cannot be empty.");
                            resultModel.setCode(Result.INVALID_DATA);
                            return resultModel;
                        }
                        if (model.Type.Trim().Equals(string.Empty))
                        {
                            Log.Info("Type name cannot be empty.");
                            resultModel.setCode(Result.INVALID_DATA);
                            return resultModel;
                        }

                        var accountAdmin = dbContext.Accounts.FirstOrDefault(m => m.Email.Trim().Equals(currentUser.Email.Trim())
                                                               && (m.RoleId == RoleEnum.Admin || m.RoleId == RoleEnum.Manager));
                        if (accountAdmin == null)
                        {
                            Log.Info("Is not Admin cannot be empty.");
                            resultModel.setCode(Result.ACCOUNT_NOT_ADMIN);
                            return resultModel;
                        }
                        #endregion

                        var sliderActive = dbContext.Sliders.Where(m => m.Id.Equals(model.Id)).FirstOrDefault();
                        if (!sliderActive.Equals(model.Type))
                        {

                            var listSlider = dbContext.Sliders.Where(m => m.Type.Equals(sliderActive.Type) && m.Status.Equals(StatusEnum.Active)).ToList();
                            sliderActive.IOrder = listSlider.Count + 1;
                        }

                        sliderActive.Title = model.Title;
                        sliderActive.Description = model.Description;
                        sliderActive.GoToLink = model.GoToLink;

                        dbContext.SaveChanges();

                        if (model.Image != null)
                        {
                            var coverImg = ImageBll.Save(model.Image, null, string.Format("_{0}_{1}", count++, sliderActive.Id));
                            if (coverImg == null)
                            {
                                Log.Error("Save image fail.");
                                resultModel.setCode(Result.FAIL_COVER);
                                return resultModel;
                            }

                            coverImg.Type = ImageTypeEnum.Cover;
                            coverImg.CreatedDate = createdDate;

                            dbContext.Images.Add(coverImg);
                            dbContext.SaveChanges();

                            sliderActive.ImageId = coverImg.Id;
                            dbContext.SaveChanges();
                        }

                        scope.Complete();
                        resultModel.setCode(Result.SUCCESS);

                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("Create Slider", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            return resultModel;
        }

        public static ResultModel getSliderById(long sliderId)
        {
            var resultModel = new ResultModel();
            try
            {
                using (var dbContext = new Entities())
                {
                    #region [Validation]

                    #endregion
                    #region [Get HomePage by Type]
                    var data = dbContext.Sliders.Where(m => m.Id.Equals(sliderId)).FirstOrDefault();

                    if (data != null)
                    {
                        SliderModel slider = new SliderModel();

                        slider.Id = data.Id;
                        slider.Title = data.Title;
                        slider.Image_Url = String.Format("/{0}/{1}/{2}", "Image", "View", data.ImageId.ToString());
                        slider.Description = data.Description;
                        slider.GoToLink = data.GoToLink;
                        slider.Status = data.Status;
                        slider.Type = data.Type;

                        resultModel.setCode(Result.SUCCESS);
                        resultModel.Data = slider;
                    }
                    #endregion
                }
            }
            catch (Exception exception)
            {
                Log.Error("getActiveSliderById", exception);
                resultModel.setCode(Result.SYSTEM);
            }

            return resultModel;
        }
    }
}