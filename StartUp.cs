using AutoMapper;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.ExportUsersAndProducts;
using ProductShop.Dtos.ExportUsersAndProducts.ExportSoldProducts;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop
{
    //original

    public class StartUp
    {

        private static string filePath;

        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile(typeof(ProductShopProfile)));
            ProductShopContext dbContext = new ProductShopContext();


            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();
            //Console.WriteLine("Database was created!");

            //InitializeFilePath("users.xml");
            // InitializeFilePath("products.xml");
            // InitializeFilePath("categories.xml");
            // InitializeFilePath("categories-products.xml");

            // InitializeOutputFilePath("products-in-range.xml");
            //InitializeOutputFilePath("users-sold-products.xml");
            //InitializeOutputFilePath("categories-by-products.xml");
            InitializeOutputFilePath("users-and-products.xml");


            //  var inputXml = File.ReadAllText(filePath);


            // Console.WriteLine(ImportUsers(dbContext, inputXml));
            // Console.WriteLine(ImportProducts(dbContext, inputXml));
            // Console.WriteLine(ImportCategories(dbContext, inputXml));
            // Console.WriteLine(ImportCategoryProducts(dbContext, inputXml));

            var xmlDocument = GetUsersWithProducts(dbContext);

            File.WriteAllText(filePath, xmlDocument);
        }

        //01 Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {

            var usersDtos = Deserialize<ImportUsersDto[]>(inputXml, "Users");

            var users = usersDtos.Select(x => Mapper.Map<User>(x)).ToArray();

            context.Users.AddRange(users);
            context.SaveChanges();


            return $"Successfully imported {users.Length}";


        }

        //02.Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var productDtos = Deserialize<ImportProductsDto[]>(inputXml, "Products");
            // var products = productDtos.Select(x => Mapper.Map<Product>(x)).ToArray();
            Product[] products = productDtos
                .Select(x => new Product
                {
                    Name = x.Name,
                    Price = x.Price,
                    SellerId = x.SellerId,
                    BuyerId = x.BuyerId

                })
                .ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        //03 Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var categoryesDto = Deserialize<ImportCategoriesDto[]>(inputXml, "Categories");
            var categories = categoryesDto
               .Where(x => x != null)
            //   .Select(x => Mapper.Map<Category>(x))
            .Select(x => new Category
            {
                Name = x.Name,

            })
               .ToArray();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Length}";

        }
        //04.ImportCategoryProducts

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var categoryProductDto = Deserialize<ImportCategoriesProductsdto[]>(inputXml, "CategoryProducts");

            var productsId = context.Products.Select(x => x.Id).ToHashSet();
            var categoriesId = context.Categories.Select(x => x.Id).ToHashSet();

            var categoriesProducts = categoryProductDto
                .Where(x => productsId.Contains(x.ProductId) && categoriesId.Contains(x.CategoryId))
               // .Select(x => Mapper.Map<CategoryProduct>(x))
               .Select(x => new CategoryProduct
               {
                   CategoryId = x.CategoryId,
                   ProductId = x.ProductId
               })
                .ToArray();

            context.CategoryProducts.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Length}";
        }
        //05 Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {

            var resultDto = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
               // .Select(x => Mapper.Map<ExportProductsInRangeDto>(x))
               .Select(x => new ExportProductsInRangeDto
               {
                   Name = x.Name,
                   Price = x.Price,
                   BuyerFullName = $"{x.Buyer.FirstName} {x.Buyer.LastName}"
               })
                .OrderBy(x => x.Price)
                .Take(10)
                .ToArray();


            return Serializer<ExportProductsInRangeDto[]>(resultDto, "Products");
        }
        //06. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var usersDto = context.Users
                    .Where(x => x.ProductsSold.Count > 0)
                    .Select(x => new ExportSoldProductsOfUserDto
                    {
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Products = x.ProductsSold.Select(a => new ExportProductDto
                        {
                            Name = a.Name,
                            Price = a.Price,

                        })
                        .ToArray()

                    })
                    .OrderBy(a => a.LastName)
                    .ThenBy(a => a.FirstName)
                    .Take(5)
                .ToArray();


            return Serializer<ExportSoldProductsOfUserDto[]>(usersDto, "Users");

        }

        //07 Export Categories by porducts count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categoriesDto = context.Categories
                    .Select(x => new ExportCategoryByProductsCount
                    {
                        Name = x.Name,
                        Count = x.CategoryProducts.Count,
                        AveragePrice = x.CategoryProducts.Average(c => c.Product.Price),
                        TotalSum = x.CategoryProducts.Sum(a => a.Product.Price)

                    })
                    .OrderByDescending(x => x.Count)
                    .ThenBy(x => x.TotalSum)
                    .ToArray();


            return Serializer<ExportCategoryByProductsCount[]>(categoriesDto, "Categories");

        }

        //08 Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersDto = context.Users      
                .ToArray()  // fix for Exception InMemory Query Internal, Test 000_001
                .Where(x => x.ProductsSold.Any())
                .OrderByDescending(x => x.ProductsSold.Count)
                .Take(10)
                .Select(x => new ExportUsersAndProductsDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,
                    SoldProducts = new ExportSoldProductsDto
                    {
                        Count = x.ProductsSold.Count,
                        Products = x.ProductsSold.Select(p => new ExportProductDto
                        {
                            Name = p.Name,
                            Price = p.Price

                        }).OrderByDescending(p => p.Price).ToArray()

                    }

                }).ToArray();


            var result = new ExportUsersAndProductsMainDto
            {
                Count = context.Users.Count(x => x.ProductsSold.Count > 0),
                Users = usersDto

            };

            return Serializer<ExportUsersAndProductsMainDto>(result, "Users");
        }

        //helpers methods

        private static T Deserialize<T>(string inputXml, string rootName)
        {
            XmlRootAttribute root = new XmlRootAttribute(rootName);
            var serializer = new XmlSerializer(typeof(T), root);


            T dtos = (T)serializer.Deserialize(new StringReader(inputXml));

            return dtos;
        }

        private static string Serializer<T>(T dto, string rootName)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute root = new XmlRootAttribute(rootName);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            var xmlSerializer = new XmlSerializer(typeof(T), root);
            using StringWriter writer = new StringWriter(sb);

            xmlSerializer.Serialize(writer, dto, namespaces);

            return sb.ToString().Trim();
        }

        private static void InitializeFilePath(string fileName)
        {
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../Datasets/", fileName);

        }
        private static void InitializeOutputFilePath(string fileName)
        {
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../Results/", fileName);
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult);

            return isValid;
        }
    }
}