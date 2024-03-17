﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Ratio_Lyrics.Web.Areas.Admin.Models;
using Ratio_Lyrics.Web.Models;
using Ratio_Lyrics.Web.Services.Abstraction;
using System.Text.Json;

namespace Ratio_Lyrics.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SongController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ISongService _songService;
        private readonly IMediaPlatformService _mediaPlatformService;
        private const int pageSizeDefault = 10;

        public SongController(IMapper mapper, ISongService songService,IMediaPlatformService mediaPlatformService)
        {
            _mapper = mapper;
            _songService = songService;
            _mediaPlatformService = mediaPlatformService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SongFilter(string? name, int? page = 1)
        {            
            if (page <= 1) page = 1;

            return RedirectToAction(nameof(Index), new { searchText = name, PageNumber = page });
        }

        public async Task<IActionResult> Index(BaseQueryParams args)
        {
            args.PageSize = pageSizeDefault;
            var songs = _mapper.Map<ListSongsAdminViewModel>(await _songService.GetSongsAsync(args));

            ViewBag.Area = "Admin";
            ViewBag.Controller = "Song";
            ViewBag.Action = "Index";

            return View(songs);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == 0) return NotFound();

            var song = await _songService.GetSongAsync(id, false);
            if (song == null) return NotFound();

            return View(song);
        }

        public async Task<IActionResult> Create()
        {
            var medias = await Task.Run(() => _mediaPlatformService.GetMediaPlatformsAsync());           
            var model = new SongViewModel
            {
                MediaPlatformLinks = _mapper.Map<List<SongMediaPlatformViewModel>>(medias),                
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SongViewModel newSong)
        {
            //demo media links
            if(!string.IsNullOrEmpty(newSong.MediaLinksForm))
                newSong.MediaPlatformLinks = JsonSerializer.Deserialize<List<SongMediaPlatformViewModel>>(newSong.MediaLinksForm);
            if(newSong.MediaPlatformLinks != null && newSong.MediaPlatformLinks.Any())
            {
                foreach (var item in newSong.MediaPlatformLinks)
                {
                    if (item.Name.Equals(Constants.CommonConstant.Spotify)) item.Link = newSong.SpotifyLink;
                    else if (item.Name.Equals(Constants.CommonConstant.Youtube)) item.Link = newSong.YoutubeLink;
                    else if (item.Name.Equals(Constants.CommonConstant.AppleMusic)) item.Link = newSong.AppleMusicLink;
                }
            }

            //demo artist
            newSong.Artists = newSong.ArtistForm?.Split(',')
                .Select(x=> new ArtistViewModel
                {
                    Name = x.Trim()
                })
                .ToList();

            var songId = await _songService.CreateSongAsync(newSong);
            if (songId != 0) return RedirectToAction(nameof(Index));

            return RedirectToAction(nameof(Create));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var song = await _songService.GetSongAsync(id, false);
            if (song == null) return NotFound();

            song.YoutubeLink = song.MediaPlatformLinks != null && song.MediaPlatformLinks.Any()
            ? song.MediaPlatformLinks.FirstOrDefault(x => x.Name.Equals(Constants.CommonConstant.Youtube))?.Link ?? string.Empty
            : string.Empty;

            song.AppleMusicLink = song.MediaPlatformLinks != null && song.MediaPlatformLinks.Any()
            ? song.MediaPlatformLinks.FirstOrDefault(x => x.Name.Equals(Constants.CommonConstant.AppleMusic))?.Link ?? string.Empty
            : string.Empty;

            song.SpotifyLink = song.MediaPlatformLinks != null && song.MediaPlatformLinks.Any()
            ? song.MediaPlatformLinks.FirstOrDefault(x => x.Name.Equals(Constants.CommonConstant.Spotify))?.Link ?? string.Empty
            : string.Empty;

            song.ArtistForm = song.Artists != null && song.Artists.Any() 
                ? string.Join(", ", song.Artists.Select(x=>x.Name))
                : string.Empty;

            return View(song);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SongViewModel model)
        {
            //demo media links
            if (!string.IsNullOrEmpty(model.MediaLinksForm))
                model.MediaPlatformLinks = JsonSerializer.Deserialize<List<SongMediaPlatformViewModel>>(model.MediaLinksForm);
            if (model.MediaPlatformLinks != null && model.MediaPlatformLinks.Any())
            {
                foreach (var item in model.MediaPlatformLinks)
                {
                    if (item.Name.Equals(Constants.CommonConstant.Spotify)) item.Link = model.SpotifyLink;
                    else if (item.Name.Equals(Constants.CommonConstant.Youtube)) item.Link = model.YoutubeLink;
                    else if (item.Name.Equals(Constants.CommonConstant.AppleMusic)) item.Link = model.AppleMusicLink;
                }
            }

            //demo artist
            model.Artists = model.ArtistForm?.Split(',')
                .Select(x => new ArtistViewModel
                {
                    Name = x.Trim()
                })
                .ToList();

            var result = await _songService.UpdateSongAsync(model);
            if (!result) return View(model);

            return RedirectToAction(nameof(Index));
        }               

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var song = await _songService.DeleteSongAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}