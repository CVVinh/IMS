using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Data;
using BE.Data.Contexts;
using BE.Helpers;
using Token = BE.Helpers.TokenHelper.Token;
using BE.Response;
using System.Net;

namespace API_LVTN.DAL
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        internal AppDbContext _context;
        internal DbSet<TEntity> _entities;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _entities = context.Set<TEntity>();
        }

        public virtual async Task<ApiRespones<IEnumerable<TEntity>>> GetListorGetListWithFilter(Token token,int? idModule, Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
        {
            var permission = GetPermission.GetPermissionForUser(token, idModule);
            if (permission.Get)
            {
                try
                {
                    IQueryable<TEntity> query = _entities;
                    if(query == null)
                    {
                        return new ApiRespones<IEnumerable<TEntity>>
                        {
                            Status = HttpStatusCode.NoContent,
                        };
                    }
                    if (filter != null)
                    {
                        query = query.Where(filter).AsNoTracking();
                    }
                    if (orderBy != null)
                    {
                        return new ApiRespones<IEnumerable<TEntity>>
                        {
                            Status = HttpStatusCode.OK,
                            Data = await orderBy(query).ToListAsync()
                        };
                    }
                    else
                    {
                        return new ApiRespones<IEnumerable<TEntity>>
                        {
                            Status = HttpStatusCode.OK,
                            Data = await query.ToListAsync()
                        };
                    }
                }
                catch(Exception ex)
                {
                    return new ApiRespones<IEnumerable<TEntity>>
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = ex.Message,
                    };
                }
            }
            else
            {
                return new ApiRespones<IEnumerable<TEntity>>
                {
                    Status = HttpStatusCode.Forbidden
                };
            }
        }

        public virtual ApiRespones<TEntity> GetById(int id,Token token, int? idModule)
        {
            var permission = GetPermission.GetPermissionForUser(token, idModule);
            if (permission.Get)
            {
                try
                {
                    var item = _entities.Find(id);
                    if (item == null)
                    {
                        return new ApiRespones<TEntity>
                        {
                            Status = HttpStatusCode.NotFound,
                        };
                    }
                    else
                    {
                        return new ApiRespones<TEntity>
                        {
                            Status = HttpStatusCode.OK,
                            Data = item
                        };
                    }
                }
                catch(Exception ex)
                {
                    return new ApiRespones<TEntity>
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = ex.Message
                    };
                }
            }
            else
            {
                return new ApiRespones<TEntity>
                {
                    Status = HttpStatusCode.Forbidden
                };
            }
        }

        public virtual ApiRespones<TEntity> GetFirstOrDefaultWithFilter(Token token, int? idModule, Expression<Func<TEntity, bool>> filter = null)
        {
            var permission = GetPermission.GetPermissionForUser(token, idModule);
            if (permission.Get)
            {
                try
                {
                    var item = _entities.AsNoTracking().FirstOrDefault();
                    if(item == null)
                    {
                        return new ApiRespones<TEntity>
                        {
                            Status = HttpStatusCode.NotFound,
                        };
                    }
                    else
                    {
                        return new ApiRespones<TEntity>
                        {
                            Status = HttpStatusCode.OK,
                            Data = item
                        };
                    }
                }
                catch(Exception ex)
                {
                    return new ApiRespones<TEntity>
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = ex.Message
                    };
                }
            }
            else
            {
                return new ApiRespones<TEntity>
                {
                    Status = HttpStatusCode.Forbidden
                };
            }
        }

        public virtual ApiRespones<TEntity> Insert(TEntity tentity, Token token, int? idModule)
        {
            var permission = GetPermission.GetPermissionForUser(token, idModule);
            if (permission.Add)
            {
                try
                {
                    _entities.Add(tentity);
                    _context.SaveChanges();
                    return new ApiRespones<TEntity>
                    {
                        Status = HttpStatusCode.Created,
                    };
                }
                catch (Exception ex)
                {
                    return new ApiRespones<TEntity>
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = ex.Message
                    };
                }
            }
            else
            {
                return new ApiRespones<TEntity>
                {
                    Status = HttpStatusCode.Forbidden
                };
            }
        }

        public virtual ApiRespones<TEntity> Update(TEntity tentity, Token token, int? idModule)
        {
            var permission = GetPermission.GetPermissionForUser(token, idModule);
            if (permission.Update)
            {
                try
                {
                    _entities.Attach(tentity);
                    _context.Entry(tentity).State = EntityState.Modified;
                    _context.SaveChanges();
                    return new ApiRespones<TEntity>
                    {
                        Status = HttpStatusCode.OK,
                    };
                }
                catch (Exception ex)
                {
                    return new ApiRespones<TEntity>
                    {
                        Status = HttpStatusCode.NotFound,
                        Message = ex.Message
                    };
                }
            }
            else
            {
                return new ApiRespones<TEntity>
                {
                    Status = HttpStatusCode.Forbidden
                };
            }
        }

        public virtual ApiRespones<TEntity> Delete(object id, Token token, int? idModule)
        {
            var permission = GetPermission.GetPermissionForUser(token, idModule);
            if (permission.Delete)
            {
                var entityToDelete = _entities.Find(id);
                try
                {
                    _entities.Attach(entityToDelete);
                    _context.Entry(entityToDelete).State = EntityState.Modified;
                    _context.SaveChanges();
                    return new ApiRespones<TEntity>
                    {
                        Status = HttpStatusCode.OK,
                    };
                }
                catch (Exception ex)
                {
                    return new ApiRespones<TEntity>
                    {
                        Status = HttpStatusCode.BadRequest,
                        Message = ex.Message
                    };
                }
            }
            else
            {
                return new ApiRespones<TEntity>
                {
                    Status = HttpStatusCode.Forbidden
                };
            }
        }

        #region Recycle Bin 
        //public virtual ApiRespones<List<TEntity>> GetByOther(Token token, int? idModule, Expression<Func<TEntity, bool>>? filter = null)
        //{
        //    var permission = GetPermission.GetPermissionForUser(token, idModule);
        //    if (permission.Get)
        //    {
        //        IQueryable<TEntity> query = _entities;
        //        if (filter != null)
        //        {
        //            query = query.Where(filter).AsNoTracking();
        //        }
        //        return new ApiRespones<List<TEntity>>
        //        {
        //            Status = HttpStatusCode.OK,
        //            Data = query.ToList()
        //        };
        //    }
        //    else
        //    {
        //        return new ApiRespones<List<TEntity>>
        //        {
        //            Status = HttpStatusCode.Forbidden
        //        };
        //    }
        //}

        //public virtual ApiRespones<TEntity> Delete(object id, int idUser, int? idModule)
        //{
        //    var permission = GetPermission.GetPermissionForUser(idUser, idModule);
        //    if (permission.Delete)
        //    {
        //        var entityToDelete = _entities.Find(id);
        //        if (entityToDelete != null)
        //        {
        //            try
        //            {
        //                _entities.Remove(entityToDelete);
        //                _context.SaveChanges();
        //                return new ApiRespones<TEntity>
        //                {
        //                    Status = HttpStatusCode.SUCCESS,
        //                };
        //            }
        //            catch (Exception ex)
        //            {
        //                return new ApiRespones<TEntity>
        //                {
        //                    Status = HttpStatusCode.ERROR,
        //                    Message = ex.Message
        //                };
        //            }
        //        }
        //        else
        //        {
        //            return new ApiRespones<TEntity>
        //            {
        //                Status = HttpStatusCode.NULL,
        //                Message = "Null"
        //            };
        //        }
        //    }
        //    else
        //    {
        //        return new ApiRespones<TEntity>
        //        {
        //            Status = HttpStatusCode.Forbidden
        //        };
        //    }
        //}
        #endregion
    }
}
