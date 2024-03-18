const inputPublicApiHost = document.getElementById("#public-api-host");
const inputSongSearch = document.getElementById("#input-search-songs");
const songDetailArea = document.getElementById("#songDetailArea");
const searchResultDemoArea = document.getElementById("#searchResultDemoArea");
const currentApiDomain = inputPublicApiHost?.value;

const GetJwtToken = () => {
  // get from cookies
  // empty => call api
  // save to cookie
  const jwt =
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKV1QgZGVtbyIsImp0aSI6IjM3MGVjYjI1LWFlMTktNDEyNi04ZmQ5LTczMGU0M2JjYmI2NSIsImlhdCI6IjE3MTA3NTY3NjgiLCJVc2VyTmFtZSI6Imh1eXJhdGlvIiwiZXhwIjoxNzEwNzYwMzY4LCJpc3MiOiJIdXlyYXRpbyIsImF1ZCI6Ikh1eSBuZ3V5ZW4ifQ.8u-4Y4zFrGOT2yyBJeAA6xxI8ZV4FXFmYUaKtzQsOC8";
  return jwt;
};

const SearchSongsByNameApi = async (name, version = 1) => {
  const result = await fetch(
    `${currentApiDomain}/v${version}/song/search?name=${name}`,
    {
      method: "Get",
      headers: new Headers({
        Authorization: `Bearer ${GetJwtToken()}`,
        "Content-Type": "application/json",
      }),
    }
  );

  if (result.ok) {
    return await result.json();
  }

  return null;
};

const GetSongByIdApi = async (id, version = 1) => {
  const result = await fetch(
    `${currentApiDomain}/v${version}/song/get?id=${id}`,
    {
      method: "Get",
      headers: new Headers({
        Authorization: `Bearer ${GetJwtToken()}`,
      }),
    }
  );

  if (result.ok) {
    return await result.json();
  }

  return null;
};

const BuildSongSearchCardItemHtml = (song) => {
  if (song == null) return;

  const artist = song.artists.join(", ");

  let songItem = `
  <div class="abc">
    <h3>${song.displayName}</h3>
    <p>${artist}</p>  
  </div>
  `;

  return songItem;
};

const BuildSongSearchResultHtml = (songs) => {
  if (songs == null || songs.totalRecords == 0 || songs.items == null) return;

  let songResults = ``;
  songs.items.forEach((song) => {
    songResults += BuildSongSearchCardItemHtml(song);
  });

  return songResults;
};

//song detail
const BuildArtistBlockHtml = (artistArray) => {
  let artist = ``;
  if (artistArray != null && artistArray.count > 0) {
    artistArray.forEach((el) => {
      artist += `${el.name}, `;
    });
  }

  let artistBlockHtml = ``;
  if (artist != "")
    artistBlockHtml += `<h3>Artist:</h3>
  <p><i>${artist}</i></p> `;

  return artistBlockHtml;
};

const BuildMediaPlatformBlockHtml = (mediaPlatformLinks) => {
  let mediaPlatformItems = ``;
  if (mediaPlatformLinks != null && mediaPlatformLinks.count > 0) {
    mediaPlatformLinks.forEach((el) => {
      if (el.link != "") {
        mediaPlatformItems += `<li><a href="${el.link}" target="_blank">${el.name}</a></li>`;
      }
    });
  }

  let mediaPlatformBlockHtml = ``;
  if (mediaPlatformItems != "")
    mediaPlatformBlockHtml += `<h3>Media Links:</h3>${mediaPlatformItems}`;

  return mediaPlatformBlockHtml;
};

const BuildSongDetailHtml = (song) => {
  let artistBlockHtml = BuildArtistBlockHtml(song.artists);
  let mediaPlatformBlockHtml = BuildMediaPlatformBlockHtml(
    song.mediaPlatformLinks
  );
  let result = `<h2>${song.displayName}</h2>
  <div>
      <img src="${song.imageUrl}" alt="${song.name}" style="height:300px;width:auto" />
  </div>          
  ${artistBlockHtml}
  ${mediaPlatformBlockHtml}    
  <div>
      <h3>Lyrics:</h3>
      <div>${song.lyric}</div>
  </div>`;
  return result;
};

const SearchSongEvent = () => {
  if (!inputSongSearch || !searchResultDemoArea) return;

  inputSongSearch.addEventListener("change", async (e) => {
    let inputValue = e.target.value.trim();
    if (inputValue.length <= 3) return;

    const data = await SearchSongsByNameApi(inputValue, 1);

    if (data == null) {
      console.log("not found");
      return;
    } else {
      searchResultDemoArea.innerHTML = BuildSongSearchResultHtml(data);
    }
  });
};
