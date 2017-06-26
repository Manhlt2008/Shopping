using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Transactions;
using System.Web;
using log4net;
using WebApplication.Lib.Dal.DbContext;
using WebApplication.Lib.Util.Constant;
using WebApplication.Models.Product;
using WebApplication.Models.Review;
using WebApplication.Models.User;

namespace WebApplication.Lib.Bll
{
    public static class ReviewBll
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public const string SortDefault = "default";
        public const string SortAccountName = "name";
        public const string SortLatest = "latest";

        public const int Asc = 1;
        public const int Desc = 0;

        public static Review WriteReview(Order order, ProductReviewModel model)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        var orderDetail = order.OrderDetails.FirstOrDefault(m => m.ProductId == model.ProductId);
                        if (orderDetail == null)
                        {
                            Log.Info("Product not found");
                            return null;
                        }

                        var review = new Review
                        {
                            AccountId = order.UserId,
                            CreatedDate = DateTime.Now,
                            OrderId = order.Id,
                            OrderDetailId = orderDetail.Id,
                            ProductId = model.ProductId,
                            ReviewMessage = model.ReviewMessage,
                            Status = StatusEnum.ReviewEnum.Status.New
                        };

                        dbContext.Reviews.Add(review);
                        dbContext.SaveChanges();

                        var reviewHistory = new ReviewHistory
                        {
                            CreadtedDate = DateTime.Now,
                            ExtraInfo = string.Empty,
                            LastActions = StatusEnum.ReviewEnum.Action.New,
                            ReviewId = review.Id,
                            LastModifiedAccountId = order.UserId
                        };

                        dbContext.ReviewHistories.Add(reviewHistory);
                        dbContext.SaveChanges();

                        scope.Complete();

                        return review;
                    }

                }
            }
            catch (Exception exception)
            {
                Log.Error("WriteReview()", exception);
            }
            return null;
        }

        public static List<Review> FindByProductId(long productId, string sort, int order, int limit, int page,
            out int totalProducts)
        {
            try
            {
                using (var dbContext = new Entities())
                {
                    var productIds = new List<long> { productId };

                    var query = productId != 0
                        ? dbContext.Reviews.Where(
                            m => productIds.Contains(m.ProductId) && m.Status == StatusEnum.ReviewEnum.Status.New)
                        : dbContext.Reviews.Where(m => m.Status == StatusEnum.ReviewEnum.Status.New);

                    sort = sort.Trim().ToLower();

                    switch (sort)
                    {
                        case SortLatest:
                            query = order == Asc
                                ? query.OrderBy(m => m.CreatedDate)
                                : query.OrderByDescending(m => m.CreatedDate);
                            break;
                        case SortAccountName:
                            query = order == Asc
                                ? query.OrderBy(m => m.Account.Firstname)
                                : query.OrderByDescending(m => m.Account.Firstname);
                            break;
                        default:
                            query = order == Asc
                                ? query.OrderBy(m => m.ProductId)
                                : query.OrderByDescending(m => m.ProductId);
                            break;
                    }

                    totalProducts = query.Count();

                    if (limit > 0)
                    {
                        query = query.Skip((page - 1) * limit).Take(limit);
                    }

                    return
                        query.Include(m => m.Account)
                            .Include(m => m.Product)
                            .Include(m => m.Order)
                            .Include(m => m.Product.Category)
                            .Include(m => m.Product.Images)
                            .ToList();

                }
            }
            catch (Exception exception)
            {
                Log.Error("FindByCategoryId", exception);
            }

            totalProducts = 0;

            return Enumerable.Empty<Review>().ToList();
        }

        internal static bool UpdateStatus(long reviewId, int status = StatusEnum.ReviewEnum.Status.Approve, UserModel executor = null)
        {
            if (!StatusEnum.ReviewEnum.Status.IsValid(status))
            {
                Log.Info("UpdateStatus()>>>Invalid status enum");
                return false;
            }

            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        var review = dbContext.Reviews.FirstOrDefault(m => m.Id == reviewId);
                        if (review == null)
                        {
                            return false;
                        }

                        review.Status = status;
                        dbContext.SaveChanges();


                        if (executor == null)
                        {
                            executor = UserBll.GetUser();
                        }

                        var history = new ReviewHistory
                        {
                            LastActions = StatusEnum.ReviewEnum.Action.Approve,
                            CreadtedDate = DateTime.Now,
                            ExtraInfo = "Update from NEW to APPROVE",
                            ReviewId = reviewId,
                            LastModifiedAccountId = executor != null ? executor.Id : 1
                        };

                        dbContext.ReviewHistories.Add(history);
                        dbContext.SaveChanges();

                        scope.Complete();
                        return true;
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("Approve", exception);
            }
            return false;
        }

        public static bool UpdateAndApprove(UpdateAndApproveSaveReviewModel reviewModel, UserModel executor)
        {
            if (executor == null)
            {
                Log.Error("Access Denied. Executor not found.");
                return false;
            }

            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        var review = dbContext.Reviews.FirstOrDefault(m => m.Id == reviewModel.ReviewId);
                        if (review == null)
                        {
                            Log.Info(string.Format("UpdateAndApprove()>>>Review [{0}] not found", reviewModel.ReviewId));
                            return false;
                        }

                        var oldMessage = review.ReviewMessage;

                        review.ReviewMessage = reviewModel.Message.Trim();
                        review.Status = StatusEnum.ReviewEnum.Status.Approve;

                        dbContext.SaveChanges();

                        var history = new ReviewHistory
                        {
                            CreadtedDate = DateTime.Now,
                            ExtraInfo = "{" +
                                        "    Info: \"Update and Approve Message\"," +
                                        "    OldMessage: \"" + oldMessage + "\"," +
                                        "    NewMessage: \"" + reviewModel.Message.Trim() + "\"" +
                                        "}",
                            LastModifiedAccountId = executor.Id,
                            ReviewId = reviewModel.ReviewId
                        };

                        dbContext.ReviewHistories.Add(history);
                        dbContext.SaveChanges();

                        scope.Complete();
                        return true;

                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error("UpdateAndApprove()", exception);
            }

            return false;
        }

        public static bool Delete(long reviewId, UserModel executor)
        {
            if (executor == null)
            {
                return false;
            }

            try
            {
                using (var dbContext = new Entities())
                {
                    using (var scope = new TransactionScope())
                    {
                        var review = dbContext.Reviews.FirstOrDefault(m => m.Id == reviewId);
                        if (review == null)
                        {
                            Log.Info(string.Format("Review with ID = {0} not found.", reviewId));
                            return false;
                        }

                        review.Status = StatusEnum.ReviewEnum.Status.Deleted;
                        dbContext.SaveChanges();

                        var history = new ReviewHistory
                        {
                            CreadtedDate = DateTime.Now,
                            ExtraInfo = "{" +
                                        "    Info: \"Delete Message\"" +
                                        "}",
                            LastModifiedAccountId = executor.Id,
                            ReviewId = reviewId
                        };

                        dbContext.ReviewHistories.Add(history);
                        dbContext.SaveChanges();

                        scope.Complete();
                    }

                }
                return true;
            }
            catch (Exception exception)
            {
                Log.Error("Delete()", exception);
            }

            return false;
        }
    }
}