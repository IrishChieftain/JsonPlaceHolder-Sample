using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace Utilant.Models
{
    public class AlbumViewModel
    {
        public string ThumbnailUrl { get; set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        [Required(ErrorMessage = "Search query is required.")]
        public string SearchQuery { get; set; }

        public string Username { get; set; }
    }


    // Could have used a repository class in a separate file for this, and time permitting,
    // considered using dependency injection. Goal was simply to get this working per spec.
    public static class AlbumsModelBAL
    {
        private const string albumsAddress = "https://jsonplaceholder.typicode.com/albums";
        private const string usersAddress = "https://jsonplaceholder.typicode.com/users";
        private const string photosAddress = "https://jsonplaceholder.typicode.com/photos";
        private const string postsAddress = "https://jsonplaceholder.typicode.com/posts";


        public static List<AlbumViewModel> GetAlbums()
        {
            List<User> users = null;
            List<AlbumModel> albums = null;
            List<PhotoModel> photos = null;

            AlbumViewModel albumVM = null;
            List<AlbumViewModel> albumsList = new List<AlbumViewModel>();

            string thumbnailURL, title, name, email, phone, address, userName = string.Empty;

            using (var client = new HttpClient())
            {
                var usersResponse = client.GetAsync(usersAddress).Result;
                var albumsResponse = client.GetAsync(albumsAddress).Result;
                var photosResponse = client.GetAsync(photosAddress).Result;

                if (usersResponse.IsSuccessStatusCode && albumsResponse.IsSuccessStatusCode && photosResponse.IsSuccessStatusCode)
                {
                    var usersResponseContent = usersResponse.Content;
                    string usersResponseString = usersResponseContent.ReadAsStringAsync().Result;
                    users = JsonConvert.DeserializeObject<List<User>>(usersResponseString);

                    var albumsResponseContent = albumsResponse.Content;
                    string albumsResponseString = albumsResponseContent.ReadAsStringAsync().Result;
                    albums = JsonConvert.DeserializeObject<List<AlbumModel>>(albumsResponseString);

                    var photosResponseContent = photosResponse.Content;
                    string photosResponseString = photosResponseContent.ReadAsStringAsync().Result;
                    photos = JsonConvert.DeserializeObject<List<PhotoModel>>(photosResponseString);

                    // https://gist.github.com/ctrl-alt-d/f3bc222ea54c7e9a3c4c9acfb7fffa5d
                    // LINQ version - returning 5,000 records. This is obviously wrong because if
                    // we only return the first thumbnail of each album, we should end up with 100 records.
                    // Time permitting, I would revise this query to do that.
                    //albumsList = (from album in albums
                    //              join photo in photos on album.Id equals photo.AlbumId
                    //              join person in users on album.UserId equals person.Id
                    //              select new AlbumViewModel
                    //              {
                    //                  ThumbnailUrl = photo.ThumbnailUrl,
                    //                  Title = album.Title,
                    //                  Name = person.Name,
                    //                  Email = person.Email,
                    //                  Phone = person.Phone,
                    //                  Address = person.Address.Street + ", " + person.Address.Suite + ", " + person.Address.City + ", " + person.Address.Zipcode
                    //              }).ToList();

                    // Start outer loop (original attempt - blatant hack)
                    for (int i = 0; i < users.Count; i++)
                    {
                        for (int j = 0; j < albums.Count; j++)
                        {
                            if (users[i].Id == albums[j].UserId)
                            {
                                for (int k = 0; k < photos.Count; k++)
                                {
                                    if (albums[j].Id == photos[k].AlbumId)
                                    {
                                        thumbnailURL = photos[k].ThumbnailUrl;
                                        title = albums[j].Title;
                                        name = users[i].Name;
                                        email = users[i].Email;
                                        phone = users[i].Phone;

                                        StringBuilder sb = new StringBuilder();
                                        sb.Append(users[i].Address.Street);
                                        sb.Append(", ");
                                        sb.Append(users[i].Address.Suite);
                                        sb.Append(", ");
                                        sb.Append(users[i].Address.City);
                                        sb.Append(", ");
                                        sb.Append(users[i].Address.Zipcode);
                                        address = sb.ToString();

                                        userName = users[i].Username;

                                        albumVM = new AlbumViewModel
                                        {
                                            ThumbnailUrl = thumbnailURL,
                                            Title = title,
                                            Name = name,
                                            Email = email,
                                            Phone = phone,
                                            Address = address,
                                            Username = userName
                                        };
                                        albumsList.Add(albumVM);
                                        break; 
                                    }
                                }
                            }
                        }
                    } 
                    // End outer loop
                }
                else
                {
                    // Handling error in controller
                    throw new Exception();
                }
            }
            return albumsList;
        }


        public static List<PostModel> GetPosts(int userID)
        {
            List<User> users = null;
            List<PostModel> posts = new List<PostModel>();

            using (var client = new HttpClient())
            {
                var usersResponse = client.GetAsync(usersAddress).Result;
                var postsResponse = client.GetAsync(postsAddress).Result;

                if (usersResponse.IsSuccessStatusCode && postsResponse.IsSuccessStatusCode)
                {
                    var usersResponseContent = usersResponse.Content;
                    string usersResponseString = usersResponseContent.ReadAsStringAsync().Result;
                    users = JsonConvert.DeserializeObject<List<User>>(usersResponseString);

                    var postsResponseContent = postsResponse.Content;
                    string photosResponseString = postsResponseContent.ReadAsStringAsync().Result;
                    posts = JsonConvert.DeserializeObject<List<PostModel>>(photosResponseString);

                    posts = (from post in posts
                                  join user in users on post.UserId equals user.Id
                                  where user.Id == userID
                                  select new PostModel
                                  {
                                      Title = post.Title,
                                      Body = post.Body
                                  }).ToList();
                }
                else
                {
                    // Handling error in controller
                    throw new Exception();
                }
            }
            return posts;
        }


        public static List<AlbumViewModel> GetThumbs(string title)
        {
            List<AlbumModel> albums = null;
            List<PhotoModel> photos = null;
            List<AlbumViewModel> albumsList = new List<AlbumViewModel>();

            using (var client = new HttpClient())
            {
                var albumsResponse = client.GetAsync(albumsAddress).Result;
                var photosResponse = client.GetAsync(photosAddress).Result;

                if (albumsResponse.IsSuccessStatusCode && photosResponse.IsSuccessStatusCode)
                {
                    var albumsResponseContent = albumsResponse.Content;
                    string albumsResponseString = albumsResponseContent.ReadAsStringAsync().Result;
                    albums = JsonConvert.DeserializeObject<List<AlbumModel>>(albumsResponseString);

                    var photosResponseContent = photosResponse.Content;
                    string photosResponseString = photosResponseContent.ReadAsStringAsync().Result;
                    photos = JsonConvert.DeserializeObject<List<PhotoModel>>(photosResponseString);

                    albumsList = (from album in albums
                                  join photo in photos on album.Id equals photo.AlbumId
                                  where album.Title == title
                                  select new AlbumViewModel
                                  {
                                      ThumbnailUrl = photo.ThumbnailUrl,
                                      Title = album.Title
                                  }).ToList();
                }
                else
                {
                    // Handling error in controller
                    throw new Exception();
                }
            }
            return albumsList;
        }


        public static UserViewModel GetUser(string email)
        {
            // https://github.com/asadikhan/MVC-Razor/blob/master/WebApplication-Razor/Models/UserModel.cs
            UserViewModel userVM = new UserViewModel();
            List<UserViewModel> usersList = new List<UserViewModel>();           

            using (var client = new HttpClient())
            {
                HttpResponseMessage usersResponse = client.GetAsync(usersAddress).Result;

                if (usersResponse.IsSuccessStatusCode)
                {
                    var usersResponseContent = usersResponse.Content;
                    string usersResponseString = usersResponseContent.ReadAsStringAsync().Result;
                    usersList = JsonConvert.DeserializeObject<List<UserViewModel>>(usersResponseString);

                    userVM = usersList.Find(x => x.Email == email); 
                }
                else
                {
                    // Handling error in controller
                    throw new Exception();
                }
            }
            return userVM;
        }


        public static List<AlbumViewModel> SearchAlbums(string query)
        {
            List<AlbumViewModel> tempList;
            List<AlbumViewModel> albumsList = new List<AlbumViewModel>();

            // Ideally I wouldn't use three feeds when only two are needed - short on time!
            tempList = GetAlbums();

            foreach (AlbumViewModel albumsVM in tempList)
                // The ignore case issue might be different if the data was coming from SQL Server (collation, etc)
                if (string.Equals(albumsVM.Username, query, StringComparison.CurrentCultureIgnoreCase) || string.Equals(albumsVM.Title, query, StringComparison.CurrentCultureIgnoreCase))
                    albumsList.Add(albumsVM);

            return albumsList;
        }
    }
}