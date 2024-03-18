const inputPublicApiHost = document.querySelector("#public-api-host");
const inputSongSearch = document.querySelector("#input-search-songs");
const songDetailArea = document.querySelector("#songDetailArea");
const searchResultDemoArea = document.querySelector("#searchResultDemoArea");
const currentApiDomain = inputPublicApiHost?.value;

//common
// param object properties: Name, Value, IsNumberAndApplyFilter, IsStringify
const UpdateUrlParams = (params) => {
  if (!params) return;

  const url = new URL(window.location.href);
  let stateData = {};

  for (let i = 0; i < params.length; i++) {
    const param = params[i];
    stateData[param.Name] = param.Value;

    // remove param on url when no valuable
    if (
      param.IsNumberAndApplyFilter &&
      (!param.Value || param.Value == 1 || param.Value == 0)
    ) {
      url.searchParams.delete(param.Name);
      continue;
    } else if (!param.IsNumberAndApplyFilter && !param.Value) {
      url.searchParams.delete(param.Name);
      continue;
    }

    // add or update param value
    const paramExist = url.searchParams.has(param.Name);
    const paramData = param.Value;

    if (paramExist) url.searchParams.set(param.Name, paramData);
    else url.searchParams.append(param.Name, paramData);
  }

  window.history.pushState(stateData, "", url);
};

const GetJwtToken = () => {
  // get from cookies
  // empty => call api
  // save to cookie
  const jwt =
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKV1QgZGVtbyIsImp0aSI6ImU3NDRiMGQxLTMwMTctNDEzNS05MjZiLTA4ZjRlYjE2MDk3ZiIsImlhdCI6IjE3MTA3OTE2MjYiLCJVc2VyTmFtZSI6Imh1eXJhdGlvIiwiZXhwIjoxNzEwNzk1MjI2LCJpc3MiOiJIdXlyYXRpbyIsImF1ZCI6Ikh1eSBuZ3V5ZW4ifQ.JCWahnTU_VR-bTx0Kwa58g41S10d2xwS6atl5UFpdGE";
  return jwt;
};

//apis
const SearchSongsByNameApi = async (name, version = 1) => {
  const result = await fetch(
    `${currentApiDomain}/api/v${version}/song/search?name=${name}`,
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
    `${currentApiDomain}/api/v${version}/song/get?id=${id}`,
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

//seach bar
const BuildSongSearchCardItemHtml = (song) => {
  if (song == null) return;

  const artist = song.artists.map((el) => el.name).join(", ");
  const imageBlock =
    song.imageUrl == ""
      ? ""
      : `<img src="${song.imageUrl}" style="height:50px;width:90px" alt="${song.name}">`;
  let songItem = `
  <div class="js_selectSearchResultEvent" data-id=${song.id}>
    <h3>${song.displayName}</h3>
    ${imageBlock}
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

const ResetSongSearchBar = () => {
  inputSongSearch.value = "";
  searchResultDemoArea.innerHTML = "";
};
//song detail
const BuildArtistBlockHtml = (artistArray) => {
  let artist = artistArray.map((el) => el.name).join(", ");
  let artistBlockHtml = ``;
  if (artist != "")
    artistBlockHtml += `<h3>Artist:</h3>
  <p><i>${artist}</i></p> `;

  return artistBlockHtml;
};

const BuildMediaPlatformBlockHtml = (mediaPlatformLinks) => {
  let mediaPlatformItems = ``;
  mediaPlatformLinks
    .filter((x) => x.link != "")
    .forEach((el) => {
      mediaPlatformItems += `<li><a href="${el.link}" target="_blank">${el.name}</a></li>`;
    });

  let mediaPlatformBlockHtml = ``;
  if (mediaPlatformItems != "")
    mediaPlatformBlockHtml += `<h3>Media Links:</h3><ul>${mediaPlatformItems}</ul>`;

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

const HandleSearchSongDisplay = async (e, searchAnyway = false) => {
  let inputValue = e.target.value.trim();
  if (!searchAnyway && inputValue.length < 3) {
    searchResultDemoArea.innerHTML = "";
    return;
  }

  const data = await SearchSongsByNameApi(inputValue, 1);
  if (data == null || data.items.length == 0 || data.totalRecords == 0) {
    searchResultDemoArea.innerHTML = `<p>Not found</p>`;
    return;
  } else {
    searchResultDemoArea.innerHTML = BuildSongSearchResultHtml(data);
    ChooseSongSearchResultEvent();
  }
};

const SearchSongEvent = () => {
  if (!inputSongSearch || !searchResultDemoArea) return;

  inputSongSearch.addEventListener("input", async (e) =>
    HandleSearchSongDisplay(e)
  );
  inputSongSearch.addEventListener("keyup", async (e) => {
    if (e.keyCode === 13) {
      HandleSearchSongDisplay(e, true);
    }
  });
};

const ChooseSongSearchResultEvent = () => {
  const searchItems = document.querySelectorAll(".js_selectSearchResultEvent");
  if (!searchItems) return;

  searchItems.forEach((el, index) => {
    const songId = +el.dataset.id;
    if (songId) {
      el.addEventListener("click", async () => {
        const song = await GetSongByIdApi(songId, 1);
        if (song == null) {
          console.log("Can't find song");
        } else {
          songDetailArea.innerHTML = BuildSongDetailHtml(song);

          //add param to url
          UpdateUrlParams([
            {
              Name: "text",
              Value: song.displayName,
              IsNumberAndApplyFilter: false,
            },
          ]);
          ResetSongSearchBar();
        }
      });
    }
  });
};

SearchSongEvent();
