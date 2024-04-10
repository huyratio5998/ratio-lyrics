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
    "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJKV1QgZGVtbyIsImp0aSI6IjIxZDY4ZmRmLTM1MTctNGYxNy05NTQzLTJmYzMzZWMyODJiOCIsImlhdCI6IjE3MTA4NDY1NDEiLCJVc2VyTmFtZSI6Imh1eXJhdGlvIiwiZXhwIjoxNzEwODUwMTQxLCJpc3MiOiJIdXlyYXRpbyIsImF1ZCI6Ikh1eSBuZ3V5ZW4ifQ.CFxxJmwxRFDypP92HFJnH4yqlqKhX1Mh6Kbiq3_taIk";
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
      : `<img src="${song.imageUrl}" style="height:50px;width:90px;margin: 10px auto;" alt="${song.name}">`;
  let songItem = `
  <div class="js_selectSearchResultEvent search-item row" style="border-bottom: ridge;" data-id=${song.id}>
      <div class="row" style="height:70px">
      <div class="col-4 col-md-2">
      ${imageBlock}
      </div>
      <div class="col-8 col-md-10">
        <h5>${song.displayName}</h5>
        <p>${artist}</p>
      </div>
    </div> 
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
  let artistBlockHtml = ``;
  if (!artistArray) return artistBlockHtml;

  let artist = artistArray.map((el) => el.name).join(", ");
  if (artist != "")
    artistBlockHtml += `<p><span class="h4">Artist: </span><span><i>${artist}</i></span> </p>`;

  return artistBlockHtml;
};

const BuildSongDescriptionBlockHtml = (description) => {
  let result = ``;
  if (!description) return result;
  const desctiptionLength = description.length;
  let descriptionDisplay = description.substring(
    0,
    desctiptionLength > 200 ? 200 : desctiptionLength
  );
  if (description != "")
    result += `<p>${descriptionDisplay}${
      desctiptionLength > 200 ? `...` : ``
    }</p>`;

  return result;
};

const BuildMediaPlatformBlockHtml = (mediaPlatformLinks) => {
  let mediaPlatformItems = ``;
  if (!mediaPlatformLinks) return mediaPlatformItems;

  mediaPlatformLinks
    .filter((x) => x.link != "")
    .forEach((el) => {
      mediaPlatformItems += `<li>
      <a href="${el.link}" target="_blank">
          <img src="${el.image}" alt="${el.name}" style="height:20px" />
          ${el.name}
      </a>
  </li>`;
    });

  let mediaPlatformBlockHtml = ``;
  if (mediaPlatformItems != "")
    mediaPlatformBlockHtml += `<div>
                                <h4>Media Links:</h4>
                                <ul>${mediaPlatformItems}</ul>
                              </div>`;

  return mediaPlatformBlockHtml;
};

const BuildSongDetailHtml = (song) => {
  let artistBlockHtml = BuildArtistBlockHtml(song.artists);
  let mediaPlatformBlockHtml = BuildMediaPlatformBlockHtml(
    song.mediaPlatformLinks
  );
  let description = BuildSongDescriptionBlockHtml(song.description);
  let result = `<div>
  <h1>${song.displayName}</h1>  
  </div>
  <div class="row">
    <div class="col-md-6">
        <div>
          <img src="${song.imageUrl}" alt="${
    song.name
  }" style="height:auto;width:100%" />
        </div>
    </div>
    <div class="col-md-6">
        ${artistBlockHtml}
        ${description}
        ${mediaPlatformBlockHtml}            
        <div>${
          song.releaseDateDisplay != ""
            ? `<div><b>Release Date</b><i>(dd-mm-yyyy): </i><span>${song.releaseDateDisplay}</span></div>`
            : ``
        }          
        </div>
        <div>
            <p><b>Views: </b><span>${song.views}</span></p>
        </div>
    </div>
</div>        
  <div>
      <p>
          <span class="h3">Lyrics:</span>
          <span>
              <i>
                  ${
                    song.contributedBy
                      ? `(contributed by ${song.contributedBy})`
                      : ""
                  }
              </i>
          </span>
      </p>      
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
    searchResultDemoArea.innerHTML = `<p>Not found.<i> Want to contribute new song</i> <button class="js_btn-contribute-song btn btn-sm btn-info mt-1" data-bs-toggle="modal" data-bs-target="#contribute-song-modal">Add new song</button></p>`;
    ContributeNewSongEvent();
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

const ContributeNewSongEvent = () => {
  const btnContributeSong = document.querySelector(".js_btn-contribute-song");
  if (!btnContributeSong) return;

  btnContributeSong.addEventListener("click", () => {
    document.querySelector("#js_contribute-song-form").reset();

    const imagePreview = document.querySelector(".js_img_changeTarget img");
    if (!imagePreview) return;

    imagePreview.src = `/images/songs/no-song.png`;
  });
};

const SaveSongContributeEvent = () => {
  const btnContributeSong = document.querySelector(
    ".js_btn-save-song-contribution"
  );
  const contributeForm = document.querySelector("#js_contribute-song-form");
  if (!btnContributeSong || !contributeForm) return;

  const errorMessageEl = document.querySelector(
    ".js_contributeSong-error-message"
  );
  btnContributeSong.addEventListener("click", async (e) => {
    e.preventDefault();

    const songName = document.querySelector(".js_ratio-name");
    const songLyrics = tinymce.get("contribute-textarea");
    console.log("songName: " + songName.value);
    console.log("songLyrics: " + songLyrics.getContent());
    if (!songName.value || !songLyrics.getContent()) {
      console.log("go inside");
      DisplayMessageInMomentWithClasses(
        errorMessageEl,
        `Name or Lyrics can not be empty!`,
        ["mt-3", "col-md-5", "alert", "alert-danger"],
        5000
      );
      return;
    }

    await grecaptcha.execute();
    contributeForm.submit();
  });
};

SearchSongEvent();
SaveSongContributeEvent();
