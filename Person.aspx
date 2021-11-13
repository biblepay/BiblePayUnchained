<%@ Page Title="View Person" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Person.aspx.cs" Inherits="Unchained.Person" %>

<%@ Register TagPrefix="BBP" Namespace="BiblePayPaginator" Assembly="BiblePayPaginator" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="Scripts/emoji/emojionearea.min.css" rel="stylesheet" />
    <link href="Content/pages/people.css" rel="stylesheet" />
    <div class="container">
        <div class="profile-page tx-13">
            <div class="row">
                <div class="col-12 grid-margin">
                    <div class="profile-header">
                        <div class="cover">
                            <div class="cover-body d-flex justify-content-between align-items-center">
                                <div>
                                    <%= user.GetAvatarImageNoDims("profile-pic") %>
                                    <span class="profile-name"><%= user.FullUserName() %></span>
                                </div>
                                <div class="d-none d-md-block">
                                    <% if (IsMe)
                                        {%>
                                    <a class="btn btn-primary btn-icon-text btn-edit-profile" href="/Profile">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-edit btn-icon-prepend">
                                            <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path>
                                            <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path>
                                        </svg>
                                        Edit profile
                                    </a>
                                    <% }  %>
                                </div>
                            </div>
                        </div>
                        <div class="header-links">
                            <ul class="links d-flex align-items-center mt-3 mt-md-0">
                                <li class="header-link-item d-flex align-items-center active">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-columns me-1 icon-md">
                                        <path d="M12 3h7a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-7m0-18H5a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h7m0-18v18"></path>
                                    </svg>
                                    <a class="pt-1px d-none d-md-block" href="#">Timeline</a>
                                </li>
                                <li class="header-link-item ms-3 ps-3 border-start d-flex align-items-center">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-user me-1 icon-md">
                                        <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"></path>
                                        <circle cx="12" cy="7" r="4"></circle>
                                    </svg>
                                    <a class="pt-1px d-none d-md-block" href="#">About</a>
                                </li>
                                <li class="header-link-item ms-3 ps-3 border-start d-flex align-items-center">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-users me-1 icon-md">
                                        <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path>
                                        <circle cx="9" cy="7" r="4"></circle>
                                        <path d="M23 21v-2a4 4 0 0 0-3-3.87"></path>
                                        <path d="M16 3.13a4 4 0 0 1 0 7.75"></path>
                                    </svg>
                                    <a class="pt-1px d-none d-md-block" href="#">Friends <span id="friendscount" class="text-muted tx-12"></span></a>
                                </li>
                                <li class="header-link-item ms-3 ps-3 border-start d-flex align-items-center">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-image me-1 icon-md">
                                        <rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect>
                                        <circle cx="8.5" cy="8.5" r="1.5"></circle>
                                        <polyline points="21 15 16 10 5 21"></polyline>
                                    </svg>
                                    <a class="pt-1px d-none d-md-block" href="#">Photos</a>
                                </li>
                                <li class="header-link-item ms-3 ps-3 border-start d-flex align-items-center">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-video me-1 icon-md">
                                        <polygon points="23 7 16 12 23 17 23 7"></polygon>
                                        <rect x="1" y="5" width="15" height="14" rx="2" ry="2"></rect>
                                    </svg>
                                    <a class="pt-1px d-none d-md-block" href="#">Videos</a>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row profile-body">
                <!-- left wrapper start -->
                <div class="d-none d-md-block col-md-4 col-xl-3 left-wrapper">
                    <div class="card rounded">
                        <div class="card-body">
                            <div class="d-flex align-items-center justify-content-between mb-2">
                                <h6 class="card-title mb-0">About</h6>
                                <% if (IsMe)
                                    {%>

                                <div class="dropdown">
                                    <button class="btn p-0" type="button" id="aboutdropdownMenuButton" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-more-horizontal icon-lg text-muted pb-3px">
                                            <circle cx="12" cy="12" r="1"></circle>
                                            <circle cx="19" cy="12" r="1"></circle>
                                            <circle cx="5" cy="12" r="1"></circle>
                                        </svg>
                                    </button>
                                    <div class="dropdown-menu" aria-labelledby="vdropdownMenuButton">
                                        <a class="dropdown-item d-flex align-items-center" href="#">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-edit-2 icon-sm me-2">
                                                <path d="M17 3a2.828 2.828 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z"></path>
                                            </svg>
                                            <span class="">Edit</span></a>
                                        <a class="dropdown-item d-flex align-items-center" href="#">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-git-branch icon-sm me-2">
                                                <line x1="6" y1="3" x2="6" y2="15"></line>
                                                <circle cx="18" cy="6" r="3"></circle>
                                                <circle cx="6" cy="18" r="3"></circle>
                                                <path d="M18 9a9 9 0 0 1-9 9"></path>
                                            </svg>
                                            <span class="">Update</span></a>
                                        <a class="dropdown-item d-flex align-items-center" href="#">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-eye icon-sm me-2">
                                                <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path>
                                                <circle cx="12" cy="12" r="3"></circle>
                                            </svg>
                                            <span class="">View all</span></a>
                                    </div>
                                </div>
                                <%} %>
                            </div>
                            <% if (!string.IsNullOrEmpty(user.PublicText))
                                { %>
                            <h6><b>Public</b></h6>
                            <p>
                                <%= user.PublicText %>
                            </p>
                            <% } %>
                            <% if (!string.IsNullOrEmpty(user.PrivateText))
                                { %>
                            <h6><b>Private/ Friends Only</b></h6>
                            <p>
                                <%= user.PrivateText %>
                            </p>
                            <% } %>
                            <% if (!string.IsNullOrEmpty(user.ProfessionalText))
                                { %>
                            <h6><b>Professional</b></h6>
                            <p>
                                <%= user.ProfessionalText %>
                            </p>
                            <% } %>
                            <% if (!string.IsNullOrEmpty(user.ReligiousText))
                                { %>
                            <h6><b>Belief</b></h6>
                            <p>
                                <%= user.ReligiousText%>
                            </p>
                            <% } %>
                            <%--  <hr />
                   <div class="mt-3">
                        <label class="tx-11 font-weight-bold mb-0 text-uppercase">Email:</label>
                        <p class="text-muted"><%= user.EmailAddress %></p>
                    </div>--%>
                            <hr />

                            <% if (!string.IsNullOrEmpty(user.TelegramLinkName))
                                {%>
                            <div class="mt-3">
                                <label class="tx-11 font-weight-bold mb-0 text-uppercase">Telegram Name:</label>
                                <p class="text-muted"><%= user.TelegramLinkName %></p>
                            </div>
                            <%} %>

                            <% if (!string.IsNullOrEmpty(user.TelegramLinkURL))
                                {%>
                            <div class="mt-3">
                                <label class="tx-11 font-weight-bold mb-0 text-uppercase">Telegram URL:</label>
                                <p class="text-muted"><%= user.TelegramLinkURL %></p>
                            </div>
                            <% } %>

                            <% if (!string.IsNullOrEmpty(user.TelegramLinkDescription))
                                {%>
                            <div class="mt-3">
                                <label class="tx-11 font-weight-bold mb-0 text-uppercase">Telegram Description</label>
                                <p class="text-muted"><%= user.TelegramLinkDescription %></p>
                            </div>
                            <% } %>
                            <%--<div class="mt-3">
                        <label class="tx-11 font-weight-bold mb-0 text-uppercase">Website:</label>
                        <p class="text-muted"><%=user. %></p>
                    </div>--%>
                            <%--<div class="mt-3 d-flex social-links">
                        <a href="javascript:;" class="btn d-flex align-items-center justify-content-center border me-2 btn-icon github">
                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-github" data-toggle="tooltip" title="" data-original-title="github.com/nobleui">
                                <path d="M9 19c-5 1.5-5-2.5-7-3m14 6v-3.87a3.37 3.37 0 0 0-.94-2.61c3.14-.35 6.44-1.54 6.44-7A5.44 5.44 0 0 0 20 4.77 5.07 5.07 0 0 0 19.91 1S18.73.65 16 2.48a13.38 13.38 0 0 0-7 0C6.27.65 5.09 1 5.09 1A5.07 5.07 0 0 0 5 4.77a5.44 5.44 0 0 0-1.5 3.78c0 5.42 3.3 6.61 6.44 7A3.37 3.37 0 0 0 9 18.13V22"></path>
                            </svg>
                        </a>
                        <a href="javascript:;" class="btn d-flex align-items-center justify-content-center border me-2 btn-icon twitter">
                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-twitter" data-toggle="tooltip" title="" data-original-title="twitter.com/nobleui">
                                <path d="M23 3a10.9 10.9 0 0 1-3.14 1.53 4.48 4.48 0 0 0-7.86 3v1A10.66 10.66 0 0 1 3 4s-4 9 5 13a11.64 11.64 0 0 1-7 2c9 5 20 0 20-11.5a4.5 4.5 0 0 0-.08-.83A7.72 7.72 0 0 0 23 3z"></path>
                            </svg>
                        </a>
                        <a href="javascript:;" class="btn d-flex align-items-center justify-content-center border me-2 btn-icon instagram">
                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-instagram" data-toggle="tooltip" title="" data-original-title="instagram.com/nobleui">
                                <rect x="2" y="2" width="20" height="20" rx="5" ry="5"></rect>
                                <path d="M16 11.37A4 4 0 1 1 12.63 8 4 4 0 0 1 16 11.37z"></path>
                                <line x1="17.5" y1="6.5" x2="17.51" y2="6.5"></line>
                            </svg>
                        </a>
                    </div>--%>
                        </div>
                    </div>
                </div>
                <!-- left wrapper end -->
                <!-- middle wrapper start -->
                <div class="col-md-8 col-xl-6 middle-wrapper">
                    <div class="row">
                        <div class="col-12 mb-2 text-center">
                            
                            <div class="btn-group" role="group" aria-label="Filter post by category">
  <input type="radio" class="btn-check" value="A"  name="rdopostcategory" id="btnradioA" autocomplete="off" checked>
  <label class="btn btn-outline-secondary cate-type" for="btnradioA">
     <span class="d-none d-md-inline">All</span>
      <i class="d-inline d-md-none fa fa-globe" style="font-size:28px" title="All"></i>
  </label>

  <input type="radio" class="btn-check" value="SC" name="rdopostcategory" id="btnradioS" autocomplete="off">
  <label class="btn btn-outline-secondary cate-type" for="btnradioS">
     <span class="d-none d-md-inline">Social</span> 
      <img src="Content/pages/sc.png" class="icon-post d-inline d-md-none" title="Social" alt="Social"/>
      <%--<i class="d-inline d-md-none fa fa-bullseye" title="Social"></i>--%>

  </label>

  <input type="radio" class="btn-check" value="BZ" name="rdopostcategory" id="btnradioB" autocomplete="off">
  <label class="btn btn-outline-secondary cate-type" for="btnradioB">
     <span class="d-none d-md-inline">Business</span> 
            <img src="Content/pages/bz.png" class="icon-post d-inline d-md-none" title="Business" alt="Business"/>
<%--<i class="d-inline d-md-none fa fa-bank" title="Business"></i>--%>

  </label>

  <input type="radio" class="btn-check"  value="SP" name="rdopostcategory" id="btnradioF" autocomplete="off">
  <label class="btn btn-outline-secondary cate-type" for="btnradioF">
     <span class="d-none d-md-inline">Faith/Spiritual</span> 
            <img src="Content/pages/sp.png" class="icon-post d-inline d-md-none" title="Spiritual" alt="Spiritual"/>
<%--<i class="d-inline d-md-none fa fa-blind" title="Faith"></i>--%>

  </label>

  <input type="radio" class="btn-check"  value="CV" name="rdopostcategory" id="btnradioC" autocomplete="off">
  <label class="btn btn-outline-secondary cate-type" for="btnradioC">
     <span class="d-none d-md-inline">Political/Activism</span> 
      <img src="Content/pages/cv.png" class="icon-post d-inline d-md-none" title="Civic" alt="Civic"/>
      <%--<i class="d-inline d-md-none fa fa-anchor" title="Civic"></i>--%>
          </label>

  <input type="radio" class="btn-check"  value="RQ" name="rdopostcategory" id="btnradioP" autocomplete="off">
  <label class="btn btn-outline-secondary cate-type" for="btnradioP">
     <span class="d-none d-md-inline">Job Request</span> 
            <img src="Content/pages/rq.png" class="icon-post d-inline d-md-none" title="Job Request" alt="Job Request"/>
          </label>
<!--

  <input type="radio" class="btn-check"  value="PR" name="rdopostcategory" id="btnradioP" autocomplete="off">
  <label class="btn btn-outline-secondary cate-type" for="btnradioP">
     <span class="d-none d-md-inline">Prayer Request</span> 
            <img src="Content/pages/sp.png" class="icon-post d-inline d-md-none" title="Prayer Request" alt="Prayer Request"/>
            <%--<i class="d-inline d-md-none fa fa-hand-paper-o" title="Request"></i>--%>

  </label>
    -->
</div>
                        </div>
                    </div>
                    <%if (IsMe)
                        {%>
                    <div class="row">
                        <div class="col-12">
                            <div class="card mb-2">
                                <div class="card-body">
                                    <div class="d-flex justify-content-between mb-2 pb-2 border-bottom">
                                        <div class="d-flex align-items-center hover-pointer flex-fill">
                                            <%= MySelf.GetAvatarImageNoDims("img-xs rounded-circle") %>
                                            <div class="ms-2 flex-fill">
                                                <p data-bs-toggle="modal" data-bs-target="#postModal" class="mb-1 mind-post">
                                                    What's on your mind, <%= user.FirstName %>?
                                                </p>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- Modal -->
                    <div class="modal fade" id="postModal" tabindex="-1" aria-labelledby="postModalLabel" aria-hidden="true">
                        <div class="modal-dialog modal-dialog-centered">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h4 class="modal-title text-center" id="postModalLabel">Create Post</h4>
                                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                </div>
                                <div class="modal-body">
                                    <div class="d-flex align-items-center hover-pointer flex-fill">
                                        <%= MySelf.GetAvatarImageNoDims("img-xs rounded-circle") %>
                                        <div class="ms-2 flex-fill">

                                            <span class=""><%= MySelf.FullUserName() %></span>
                                            <div>
                                            <select id="visibility" class="form-control postprivacy">
                                                <option value="public">Public</option>
                                                <option value="Friends">Friends Only</option>
                                                <option value="private">Private</option>
                                                <option value="me">Only Me</option>
                                            </select>
                                                <span class="small">Type:</span>
                                             <select id="category" style="width:auto; min-width:100px;" class="form-control postprivacy">
                                                <option value="SC">SC - Social</option>
                                                <option value="BZ">BZ - Business</option>
                                                <option value="SP">SP - Faith-Spiritual-Religious</option>
                                                <option value="CV">CV - Political/Activism</option> 
                                                <option value="RQ">RQ - Job Request</option>
                                                <!--<option value="PR">PR - Prayer Request</option>-->

                                            </select>
                                                </div>
                                        </div>
                                    </div>
                                    <div class="d-flex flex-fill pt-2">
                                        <textarea class="form-control newpost" name="newpost" id="newpost" rows="3" placeholder="What's on your mind, <%= user.FirstName %>?"></textarea>

                                    </div>
                                    <div class="d-flex-fill mt-2 border position-relative posturl-content" id="url-content" style="display: none">
                                        <div class="row align-items-center ">
                                            <div class="col-md-4">
                                                <img class="img-fluid float-none d-md-flex align-self-center m-auto justify-content-md-center align-items-md-center" loading="lazy">
                                            </div>
                                            <div class="col">
                                                <h3></h3>
                                                <p></p>
                                            </div>
                                            <a href="#" target="_blank" class="stretched-link"></a>
                                        </div>
                                    </div>
                                    <div class="">
                                        <telerik:RadAsyncUpload  runat="server"  RenderMode="Classic" HideFileInput="true"
                     ID="AsyncUpload1" MultipleFileSelection="Automatic" 
                    AllowedFileExtensions=".jpeg,.jpg,.bmp,.svg,.png,.pdf,.gif,.mp3,.webm,.mp4" >
                                            <Localization Select="Add Media" />
                                        </telerik:RadAsyncUpload>
                                    </div>
                                </div>
                                <div class="modal-footer">

                                    <div class="mb-2" id="response"></div>
                                    <button type="button" onclick="savepost()" class="btn btn-primary w-100 text-center">Post</button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%} %>

                    <div class="row" id="post-container">
                    </div>

                    <div class="card" id="no-post">
                        <div class="card-body">
                            <% if (IsMe)
                                {%>
                            <p>There are no posts to show of this type.</p>
                            <%}
                                else
                                { %>
                            <p> <%=user.FirstName %> has no posts to show of this type.</p>
                            <%} %>
                        </div>
                    </div>

                    
                </div>
                <!-- middle wrapper end -->
                <!-- right wrapper start -->
                <div class="d-none d-xl-block col-xl-3 right-wrapper">
                    <div class="row">
                        <div class="col-md-12 grid-margin">
                            <div class="card rounded">
                                <div class="card-body">
                                    <h6 class="card-title">Latest Photos</h6>
                                    <div class="latest-photos">
                                        <div class="row" id="latest-images">
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 grid-margin">
                            <div class="card rounded">
                                <div class="card-body">
                                    <h6 class="card-title">Friends</h6>
                                    <div id="friends-container">
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- right wrapper end -->
            </div>
        </div>
        <div id="post-template" style="display: none;">

            <div class="col-md-12 grid-margin singlepost" id="">
                <div class="card rounded">
                    <div class="card-header">
                        <div class="d-flex align-items-center justify-content-between">
                            <div class="d-flex align-items-center">
                                <img class="img-xs rounded-circle profilepic" src="{{userpic}}" alt="">
                                <div class="ms-2">
                                    <p class="mb-0 username">{{user}}</p>
                                    <p class="tx-11 text-muted mb-0 posttime">{{time}}</p>
                                </div>
                            </div>
                            <% if (IsMe)
                                {%>
                            <div class="dropdown mainpostaction">
                                <button class="btn p-0" type="button" id="postdropdownMenuButton" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-more-horizontal icon-lg pb-3px">
                                        <circle cx="12" cy="12" r="1"></circle>
                                        <circle cx="19" cy="12" r="1"></circle>
                                        <circle cx="5" cy="12" r="1"></circle>
                                    </svg>
                                </button>
                                <div class="dropdown-menu" aria-labelledby="postdropdownMenuButton">
                                    <a class="dropdown-item d-flex align-items-center editpost" href="javascript:;">
                                        <i class="fa fa-pencil-square me-2"></i>
                                        <span class="">Edit</span></a>
                                    <a class="dropdown-item d-flex align-items-center deletepost" href="javascript:;">
                                        <i class="fa fa-trash-alt  me-2"></i>
                                        <span class="">Delete</span></a>

                                </div>
                            </div>
                            <% } %>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="post-content">
                            <p class="mb-3 tx-14 postbody">{{body}}</p>
                        </div>
                        <div class="post-attachment">
                        </div>

                        <div class="d-flex-fill mt-2 border position-relative posturl-content" id="{{contentid}}" style="display: none">
                            <div class="row align-items-center ">
                                <div class="col-md-4 border-end p-1">
                                    <img class="urlimage img-fluid float-none d-md-flex align-self-center m-auto justify-content-md-center align-items-md-center" loading="lazy">
                                </div>
                                <div class="col">
                                    <h3 class="urltitle"></h3>
                                    <p class="urldescription"></p>
                                </div>
                                <a href="#" target="_blank" class="urllink stretched-link"></a>
                            </div>
                        </div>
                        <div class="row">
                            <div class="row attachment-container mt-2">

                                </div>
                            <div class="col-12 small ">
                            <hr class="my-2" />
                                <span class="postlikecount">
                                    <i class="fa fa-thumbs-up" aria-hidden="true"></i> <span class="count"> 0 </span> Like this
                                </span>
                                <%--<span class="postdislikecount">
                                    <i class="fa fa-thumbs-up" aria-hidden="true"></i> <span class="count"></span> Dislike this
                                </span>--%>
                                
                            </div>
                        </div>
                    </div>
                    <div class="card-footer">
                        <div class="d-flex post-actions flex-fill">
                            <a href="javascript:;" class="d-flex align-items-center text-muted me-4 btnpostlike">
                                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-heart icon-md">
                                    <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"></path>
                                </svg>
                                <p class="d-none d-md-block ms-2 mb-0">Like</p>
                            </a>
                            <a href="javascript:;" class="commentbtn d-flex align-items-center text-muted me-4">
                                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-message-square icon-md">
                                    <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"></path>
                                </svg>
                                <p class="d-none d-md-block ms-2 mb-0">Comment</p>
                            </a>
                            <a href="javascript:;" class="d-flex align-items-center text-muted">
                                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="feather feather-share icon-md">
                                    <path d="M4 12v8a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2v-8"></path>
                                    <polyline points="16 6 12 2 8 6"></polyline>
                                    <line x1="12" y1="2" x2="12" y2="15"></line>
                                </svg>
                                <p class="d-none d-md-block ms-2 mb-0">Share</p>
                            </a>
                        </div>
                        <div class="post-comments new-comment g-0 row">
                            <hr class="col-12 my-2" />
                            
                            <div class="postcommentscontainer mb-2">
                            </div>
                            <div class="col" style="max-width: 45px" id="">
                                <%= MySelf.GetAvatarImageNoDims("img-xs rounded-circle commenterpic") %>
                            </div>
                            <div class="col commententrycontainer">
                                <textarea class="mb-1 writecomment" rows="1" placeholder="Write a public comment."></textarea>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div id="comment-template" class="d-none">

            <div class="row g-0 align-items-start singlecomment mt-3">
                <div class="col" style="max-width: 45px">
                    <img src="images/emptyavatar.png" class="img-xs rounded-circle commenterpic" />
                </div>
                <div class="col">
                    <p class="mb-0 commentview bg-default small position-relative">
                        <span class="saving-comment">
                            <i class="fa fa-sun fa-spin text-blue"></i>
                        </span>
                        <b class="d-block commentername"></b>
                        <span class="commentbody"></span>
                        <span class="likebox likecount" style="display:none">
                            <i class="fa fa-thumbs-up"></i>
                            <span class="count mr-1"></span>
                        </span>
                        <span class="likebox dislikecount" style="display:none">
                            <i class="fa fa-thumbs-down"></i>
                            <span class="count mr-1"></span>
                        </span>
                    </p>

                </div>
                <div class="col-8 w-100 ps-4 ms-4 small comment-actions">

                    <span class="commenttime me-2"></span>
                    <a href="javascript:;" class="commentlike me-2"><i class="fa fa-thumbs-up" aria-hidden="true"></i>Like
                    </a>
                    <a href="javascript:;" class="commentdis me-2"><i class="fa fa-thumbs-down" aria-hidden="true"></i>Dislike
                    </a>
                    <%if (IsMe)
                        {%>
                    <a href="javascript:;" class="commentedit me-2"><i class="fa fa-pencil-square-o" aria-hidden="true"></i>Edit
                    </a>
                    <a href="javascript:;" class="commentdel"><i class="fa fa-trash me-1" aria-hidden="true"></i>Delete
                    </a>
                    <% } %>
                </div>
            </div>
        </div>
        <div id="friend-template" style="display: none;">
            <div class="d-flex justify-content-between mb-2 pb-2 border-bottom  position-relative">
                <div class="d-flex align-items-center hover-pointer">
                    <img class="img-xs rounded-circle" src="{{userpic}}" alt="">
                    <div class="ms-2">
                        <p class="mb-0">{{user}}</p>
                        <%--<p class="tx-11 text-muted">12 Mutual Friends</p>--%>
                    </div>
                </div>
                <%if (IsMe)
                    {%>
                <button class="btn btn-icon">
                    <i class="fa fa-heart-broken"></i>
                </button>
                <%} %>
                <a href="/Person?id={{userid}}" class="stretched-link"></a>
            </div>
        </div>


         <!-- Modal -->
                    <div class="modal fade" id="contentpopup" tabindex="-1"  aria-hidden="true">
                        <div class="modal-dialog modal-dialog-centered modal-fullscreen">
                            <div class="modal-content">
                                <div class="modal-body py-0">    
                              <div class="row">
                                 
                                       
                                  <div class="col bg-dark">
                                      <img src="" alt="" class="img-fluid tosee" />
                                  </div>
                                  <div class="col-12 col-md-3 order-0 order-md-1 pt-2" style="max-width:300px;">
                                      <div class="d-flex align-items-center hover-pointer">
                                        <img class="img-xs rounded-circle userimage" src="" alt="">
                                        <div class="ms-2">
                                            <p class="mb-0 username"></p>
                                            <span class="text-muted time"></span>
                                        </div>
                                    </div>
                                      <button type="button" class="btn-close position-absolute" data-bs-dismiss="modal" aria-label="Close"></button>
                               <div class="row small">
                                   <div class="col-12 mt-3">
                                       <span class="likecount" >
                            <i class="fa fa-thumbs-up"></i>
                            <span class="count mr-1">0</span> Like this
                        </span>
                                      <%-- <span class="viewcount float-end" >
                            <i class="fa fa-eye"></i>
                            <span class="count mr-1">0</span> View(s)
                        </span>--%>
                                   </div>
                                   <hr class="col-12 mt-2 mb-2" />
                                   <div class="col">
                                       <a href="javascript:;" class="btnlike me-2">
                                           <i class="fa fa-thumbs-up"></i> Like
                                       </a>
                                         <a href="javascript:;" class="btncomment me-2">  <i class="fa fa-comment"></i> Comment</a>
                                       <a href="javascript:;" class="btnshare me-2">
                                           <i class="fa fa-share"></i> Share</a>
                                   </div>
                                   <hr class="col-12 mt-2" />

                               </div>
                                  
                                   <div class="post-comments new-comment g-0 row">
                            <hr class="col-12 my-2" />
                            
                            <div class="postcommentscontainer mb-2">
                            </div>
                            <div class="col" style="max-width: 45px" id="">
                                <%= MySelf.GetAvatarImageNoDims("img-xs rounded-circle commenterpic") %>
                            </div>
                            <div class="col commententrycontainer">
                                <textarea class="mb-1 writecomment" rows="1" placeholder="Write a public comment."></textarea>
                            </div>
                        </div>
                                      <div class="row">
                                          <div id="comment-container" class="col-12">

                                          </div>
                                      </div>
                                  </div>
                              </div>
                                </div>
                                <div class="loader1">
                                    <i class="fa fa-sun fa-spin fa-4x"></i>
                                </div>
                            </div>
                        </div>
                    </div>
        <script src="Scripts/waypoint/jquery.waypoints.min.js"></script>
        <script src="Scripts/waypoint/shortcuts/infinite.min.js"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.29.1/moment.min.js" integrity="sha512-qTXRIMyZIFb8iQcfjXWCO8+M5Tbc38Qi5WzdPOYZHIlZpzBHG3L3by84BBBOiRGiEb7KKtAOAs5qYdUiZiQNNQ==" crossorigin="anonymous" referrerpolicy="no-referrer"></script>
        <script src="Scripts/emoji/emojionearea.min.js"></script>
        <script>

            $(function () {

                let visi = localStorage.getItem("post_visitbility");
                if (visi & visi != '')
                    $('#visibility').val(visi);


            });
            <%if (IsMe)
            {%>
            $("#newpost").emojioneArea({
                autoHideFilters: true,
                useSprite: true
            });
            let url = '';
            let urlfetchd = false;
            $('#newpost').emojioneArea()[0].emojioneArea.on('keyup', function (editor, event) {
                let body = editor[0].textContent;// $('#newpost').emojioneArea()[0].emojioneArea.getText();
                if (body === '') {
                    url = ''
                    $('#url-content img').attr('src', '');
                    $('#url-content h3').html('');
                    $('#url-content p').html('');
                    $('#url-content a').attr('href', '#');
                    $('#url-content').hide();
                    urlfetchd = false;
                    return;
                }
                else {

                    var urlRegex = /(\b(https?|ftp|http):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/ig;
                    // Filtering URL from the content using regular expressions
                    let urls = body.match(urlRegex);
                    console.log(urls, urlfetchd, body);
                    if (urls == null) {
                        url = ''
                        $('#url-content img').attr('src', '');
                        $('#url-content h3').html('');
                        $('#url-content p').html('');
                        $('#url-content a').attr('href', '#');
                        $('#url-content').hide();
                        urlfetchd = false;
                        return false;
                    }

                    if (!urlfetchd) {
                        if (urls.length > 0) {
                            url = urls[0];
                            let url1 = "api/web/scrap?url=" + encodeURIComponent(urls[0]);

                            $.post(url1, function (v) {
                                $('#url-content img').attr('src', v.image);
                                $('#url-content h3').html(v.title);
                                $('#url-content p').html(v.description);
                                $('#url-content').show();
                                $('#url-content a').attr('href', url);

                            });
                            urlfetchd = true;
                        }
                        else if (urlfetchd) {
                            urlfetchd = false;
                        }
                    }
                    else {
                        if (body != '' & urls.length == 0) {
                            url = ''
                            $('#url-content img').attr('src', '');
                            $('#url-content h3').html('');
                            $('#url-content p').html('');
                            $('#url-content').hide();
                            $('#url-content a').attr('href', '#');
                            urlfetchd = false;
                            return;
                        }
                    }
                }
            });
            function savepost() {
                let body = $('#newpost').emojioneArea()[0].emojioneArea.getText();
                if (body == '' & getAttachmentCount()==0) {
                    return false;
                } else {
                    body = XSS(escape(body));
                    var e = {};
                    if (url != '') {
                        e.URL = encodeURIComponent(url);
                        e.URLTitle = XSS(escape($('#url-content h3').html()));
                        e.URLDescription = XSS(escape($('#url-content p').html()));
                        e.URLPreviewImage = encodeURIComponent($('#url-content img').attr('src'));
                    }
                    e.Event = 'AddTimeline_Click';
                    e.Value = sID;
                    e.Privacy = $('#visibility').val();

                    localStorage.setItem("post_visitbility", e.Privacy);
                    e.Category = $('#category').val();
                    e.Body = body; //window.btoa(escape(body));
                    BBPPostBack2(null, e);

                }
            }

            function getAttachmentCount() {
                var upload = $find("<%=AsyncUpload1.ClientID%>");
                return upload.getUploadedFiles().length;
            }
            <%}%>



            let pno = 0;
            let me = '<%= IsMe %>';
            let homogenized = '<%= fHomogenized %>';
            let isTestNet = '<%= IsTestNet %>';
            let sID = '<%=MySelf.id%>';
            let userId = '<%=user.id%>';

            let postfinished = false;
            let loadingpost = false;

            $('input[name="rdopostcategory"]').on('change', function () {
                postfinished = false;
                loadingpost = false;
                pno = 0;
                $('#post-container').html('');
                getpost();
            });
            function getpost() {
                if (loadingpost || postfinished)
                    return false;
               let category = $('input[name="rdopostcategory"]:checked').val();
                loadingpost = true;
                $.get(`api/post/posts?category=${category}&sID=${userId}&fHomogenized=${homogenized}&me=${me}&IsTestNet=${isTestNet}&pno=${pno}`, function (response) {
                    if (response.length == 0) {
                        $('#no-post').show();
                        postfinished = true;
                        if (waypoint) {
                            waypoint.destroy();
                        }
                    }
                    else {
                        $('#no-post').hide();
                        pno += 1;
                    }
                    $.each(response, function (i, v, a) {
                        let template = $('#post-template').html().toString();
                        var html = $(template);
                        $(html[0]).attr('id', 'post-' + v.id)
                        let p = '';
                        if (v.ProfilePicture.startsWith('<img'))
                            p = $(v.ProfilePicture).attr('src');
                        else {
                            p = v.ProfilePicture;
                        }
                        let commentid = 'comment-' + v.id;
                        html.find('.profilepic').attr('src', p);
                        html.find('.username').html(v.FullName);
                        html.find('.posttime').html(PrepareTime(v.PostedOn));
                        if (v.Body == null) v.Body = "";
                        html.find('.postbody').html(v.Body?.replaceAll('\n', '<br\>'));
                        html.find('.postlikecount .count').html(v.Likes.nUpvotes);
                        html.find('.btnpostlike').on('click', function () {
                            let param = 'upvote|' + v.id + '|Timeline';
                            html.find('.postlikecount').addClass('saving')
                            $.ajax({
                                type: "POST",
                                url: "Person.aspx/voting",
                                data: { data: param },
                                headers: { headeraction: param },
                                success: function (response) {
                                    var result = JSON.parse(response);
                                    if (result.status.status) {
                                        html.find('.postlikecount .count').html(result.nUpvotes);
                                        if (result.nUpvotes > 0) {
                                            html.find('.postlikecount').show()
                                        }
                                        else {
                                            html.find('.postlikecount').hide()
                                        }
                                        updateNotificationCount();
                                        //chtml.find('.commentview .dislikecount .count').html(result.nDownvotes);
                                        //if (result.nDownvotes > 0) {
                                        //    chtml.find('.commentview .dislikecount').show()
                                        //}
                                        //else {
                                        //    chtml.find('.commentview .dislikecount').hide()
                                        //    chtml.find('.commentview .likecount').addClass('no-dislike')//'right', '10px !important');
                                        //}
                                        html.find('.postlikecount').removeClass('saving')
                                    }
                                }
                            });

                        })

                        html.find('.commentbtn').on('click', function () {
                            console.log(emojin[0].getBoundingClientRect().top);
                            //console.log($(emojin[0].getBoundingClientRect().top).offset().top);
                            emojin.data("emojioneArea").setFocus();
                        });
                        html.find('.singlecomment').attr('id', commentid);
                        let emojin = html.find('.writecomment').emojioneArea({
                            autoHideFilters: true,
                            useSprite: true,
                            pickerPosition: "bottom",
                            events: {
                                keypress: function (e, o) {
                                    if (o.originalEvent.charCode == 13) {
                                        if (!o.originalEvent.shiftKey) {
                                            o.originalEvent.returnValue = false;
                                            let body = emojin.data("emojioneArea").getText();
                                            if (body != null & body != 'undefined' & body != '') {
                                                emojin.data("emojioneArea").setText('');
                                                let ctemplate = $('#comment-template').html().toString();
                                                var chtml = $(ctemplate);
                                                let pic = $('.new-comment .commenterpic').attr('src')

                                                chtml.find('.commenterpic').attr('src', pic);
                                                chtml.find('.commentername').html('<%= MySelf.FullUserName()  %>');
                                              chtml.find('.commenttime').html(PrepareTime(new Date()));
                                              chtml.find('.commentbody').html(body?.replaceAll('\n', '<br\>'));
                                              html.find('.postcommentscontainer').append(chtml);
                                              chtml.addClass('saving');
                                              console.log(body);
                                              let data1 = JSON.stringify({ Body: XSS(escape(body)), ParentID: v.id });
                                              $.ajax({
                                                  type: "POST",
                                                  url: "Person.aspx/comment",
                                                  data: { data: data1 },
                                                  headers: { headeraction: data1 },
                                                  success: function (response) {
                                                      var result = JSON.parse(response);
                                                      if (result.status) {
                                                          updateNotificationCount();
                                                          let ncid = result.data;
                                                          $(chtml[0]).attr('id', 'comment' + ncid);
                                                          chtml.removeClass('saving');

                                                          //$(chtml[0]).attr('id', 'comment' + id);
                                                          chtml.find('.commentlike').on('click', function () {
                                                              let param = 'upvote|' + ncid + "|comment1";
                                                              chtml.addClass('saving')
                                                              $.ajax({
                                                                  type: "POST",
                                                                  url: "Person.aspx/voting",
                                                                  data: { data: param },
                                                                  headers: { headeraction: param },
                                                                  success: function (response) {
                                                                      var result = JSON.parse(response);
                                                                      if (result.status.status) {
                                                                          chtml.find('.commentview .likecount .count').html(result.nUpvotes);
                                                                          if (result.nUpvotes > 0) {
                                                                              chtml.find('.commentview .likecount').show()
                                                                          }
                                                                          else {
                                                                              chtml.find('.commentview .likecount').hide()
                                                                          }
                                                                          chtml.find('.commentview .dislikecount .count').html(result.nDownvotes);
                                                                          if (result.nDownvotes > 0) {
                                                                              chtml.find('.commentview .dislikecount').show()
                                                                          }
                                                                          else {
                                                                              chtml.find('.commentview .dislikecount').hide()
                                                                              chtml.find('.commentview .likecount').addClass('no-dislike')//'right', '10px !important');
                                                                          }
                                                                          chtml.removeClass('saving')
                                                                      }
                                                                  }
                                                              });

                                                          })
                                                          chtml.find('.commentdis').on('click', function () {
                                                              let param = 'downvote|' + ncid + "|comment1";
                                                              chtml.addClass('saving')
                                                              $.ajax({
                                                                  type: "POST",
                                                                  url: "Person.aspx/voting",
                                                                  data: { data: param },
                                                                  headers: { headeraction: param },
                                                                  success: function (response) {
                                                                      var result = JSON.parse(response);
                                                                      if (result.status) {
                                                                          chtml.find('.commentview .likecount .count').html(result.nUpvotes);
                                                                          if (result.nUpvotes > 0) {
                                                                              chtml.find('.commentview .likecount').show()
                                                                          }
                                                                          else {
                                                                              chtml.find('.commentview .likecount').hide()
                                                                          }
                                                                          chtml.find('.commentview .dislikecount .count').html(result.nDownvotes);
                                                                          if (result.nDownvotes > 0) {
                                                                              chtml.find('.commentview .dislikecount').show()
                                                                          }
                                                                          else {
                                                                              chtml.find('.commentview .dislikecount').hide()
                                                                              chtml.find('.commentview .likecount').addClass('no-dislike')//'right', '10px !important');
                                                                          }
                                                                          chtml.removeClass('saving')
                                                                      }
                                                                  }
                                                              });

                                                          })
                                                           
                                                              chtml.find('.commentdel').on('click', function () {
                                                                  chtml.addClass('saving')
                                                                  $.ajax({
                                                                      type: "POST",
                                                                      url: "Person.aspx/deletecomentbyid",
                                                                      data: { data: ncid },
                                                                      headers: { headeraction: ncid },
                                                                      success: function (response) {
                                                                          var result = JSON.parse(response);
                                                                          if (result.status) {
                                                                              chtml.remove();
                                                                          }
                                                                      }
                                                                  });
                                                              })
                                                              chtml.find('.commentedit').on('click', function () {
                                                                  if ($(this).hasClass('cancel')) {
                                                                      chtml.find('.commentbody').data('emojioneArea').disable();
                                                                      $(this).removeClass('cancel')
                                                                  }
                                                                  else {
                                                                      chtml.find('.commentbody').data('emojioneArea').enable();
                                                                      $(this).addClass('cancel')
                                                                  }
                                                              })
                                                              chtml.find('.commentbody').emojioneArea(
                                                                  {
                                                                      autoHideFilters: true,
                                                                      useSprite: true,
                                                                      pickerPosition: "bottom",
                                                                      events: {
                                                                          keypress: function (e, o) {
                                                                              if (o.originalEvent.charCode == 13) {
                                                                                  if (!o.originalEvent.shiftKey) {
                                                                                      o.originalEvent.returnValue = false;
                                                                                      let body = chtml.find('.commentbody').data("emojioneArea").getText();
                                                                                      if (body != null & body != 'undefined' & body != '') {
                                                                                          chtml.addClass('saving');
                                                                                          let data1 = JSON.stringify({ Body: XSS(escape(body)), Id: ncid });
                                                                                          $.ajax({
                                                                                              type: "POST",
                                                                                              url: "Person.aspx/editcomentbyid",
                                                                                              data: { data: data1 },
                                                                                              headers: { headeraction: data1 },
                                                                                              success: function (response) {
                                                                                                  var result = JSON.parse(response);
                                                                                                  if (result.status) {
                                                                                                      chtml.find('.commentbody').data("emojioneArea").disable();
                                                                                                      //$(chtml[0]).attr('id', 'comment' + result.data);
                                                                                                      chtml.removeClass('saving')
                                                                                                  }
                                                                                              }
                                                                                          });
                                                                                      }
                                                                                      return false;

                                                                                  }
                                                                                  else {

                                                                                  }
                                                                              }

                                                                          }
                                                                      }
                                                                  }
                                                              );
                                                              chtml.find('.commentbody').data('emojioneArea').disable();
                                                          
                                                          ///end of comment buttons copied
                                                      }
                                                  }
                                              });
                                          }
                                          return false;

                                      }
                                      else {

                                      }
                                  }

                              }
                          }
                      });


                        //writecomment
                        //template = template.replaceAll('{{body}}', v.Body.replaceAll('\n', '<br\>'));
                        if (v.URL != null & v.URL != '') {
                            //   
                            html.find('.posturl-content img.urlimage').attr('src', decodeURIComponent(v.URLPreviewImage));
                            html.find('.posturl-content .urltitle').html(v.URLTitle);
                            html.find('.posturl-content .urldescription').html(v.URLDescription);
                            html.find('.posturl-content a.urllink').attr('href', decodeURIComponent(v.URL));
                            html.find('.posturl-content').show();
                        }

                        if (v.Comments && v.Comments.length > 0) {
                            $.each(v.Comments, function (i, v1, a) {
                                let ctemplate = $('#comment-template').html().toString();
                                var chtml = $(ctemplate);
                                let p1 = '';
                                if (v1.ProfilePicture.startsWith('<img'))
                                    p1 = $(v1.ProfilePicture).attr('src');
                                else {
                                    p1 = v1.ProfilePicture;
                                }
                                chtml.find('.commenterpic').attr('src', p1);
                                chtml.find('.commentername').html(v1.FullName);
                                chtml.find('.commenttime').html(PrepareTime(v1.PostedOn));
                                chtml.find('.commentbody').html(v1.Body?.replaceAll('\n', '<br\>'));

                                //Count.nUpvotes nDownvotes
                                chtml.find('.commentview .likecount .count').html(v1.Count.nUpvotes);
                                if (v1.Count.nUpvotes > 0) {
                                    chtml.find('.commentview .likecount').show()
                                }
                                else {
                                    chtml.find('.commentview .likecount').hide()
                                }
                                chtml.find('.commentview .dislikecount .count').html(v1.Count.nDownvotes);
                                if (v1.Count.nDownvotes > 0) {
                                    chtml.find('.commentview .dislikecount').show()
                                }
                                else {
                                    chtml.find('.commentview .dislikecount').hide()
                                    chtml.find('.commentview .likecount').addClass('no-dislike')//'right', '10px !important');
                                }
                                $(chtml[0]).attr('id', 'comment' + v1.id);
                                chtml.find('.commentlike').on('click', function () {
                                    let param = 'upvote|' + v1.id + "|comment1";
                                    chtml.addClass('saving')
                                    $.ajax({
                                        type: "POST",
                                        url: "Person.aspx/voting",
                                        data: { data: param },
                                        headers: { headeraction: param },
                                        success: function (response) {
                                            var result = JSON.parse(response);
                                            if (result.status.status) {
                                                chtml.find('.commentview .likecount .count').html(result.nUpvotes);
                                                if (result.nUpvotes > 0) {
                                                    chtml.find('.commentview .likecount').show()
                                                }
                                                else {
                                                    chtml.find('.commentview .likecount').hide()
                                                }
                                                chtml.find('.commentview .dislikecount .count').html(result.nDownvotes);
                                                if (result.nDownvotes > 0) {
                                                    chtml.find('.commentview .dislikecount').show()
                                                }
                                                else {
                                                    chtml.find('.commentview .dislikecount').hide()
                                                    chtml.find('.commentview .likecount').addClass('no-dislike')//'right', '10px !important');
                                                }
                                                chtml.removeClass('saving')
                                            }
                                        }
                                    });

                                })
                                chtml.find('.commentdis').on('click', function () {
                                    let param = 'downvote|' + v1.id;
                                    chtml.addClass('saving')
                                    $.ajax({
                                        type: "POST",
                                        url: "Person.aspx/voting",
                                        data: { data: param },
                                        headers: { headeraction: param },
                                        success: function (response) {
                                            var result = JSON.parse(response);
                                            if (result.status) {
                                                chtml.find('.commentview .likecount .count').html(result.nUpvotes);
                                                if (result.nUpvotes > 0) {
                                                    chtml.find('.commentview .likecount').show()
                                                }
                                                else {
                                                    chtml.find('.commentview .likecount').hide()
                                                }
                                                chtml.find('.commentview .dislikecount .count').html(result.nDownvotes);
                                                if (result.nDownvotes > 0) {
                                                    chtml.find('.commentview .dislikecount').show()
                                                }
                                                else {
                                                    chtml.find('.commentview .dislikecount').hide()
                                                    chtml.find('.commentview .likecount').addClass('no-dislike')//'right', '10px !important');
                                                }
                                                chtml.removeClass('saving')
                                            }
                                        }
                                    });

                                })
                                if (v1.UserId == sID) {
                                    chtml.find('.commentdel').on('click', function () {
                                        chtml.addClass('saving')
                                        $.ajax({
                                            type: "POST",
                                            url: "Person.aspx/deletecomentbyid",
                                            data: { data: v1.id },
                                            headers: { headeraction: v1.id },
                                            success: function (response) {
                                                var result = JSON.parse(response);
                                                if (result.status) {
                                                    chtml.remove();
                                                }
                                            }
                                        });
                                    })
                                    chtml.find('.commentedit').on('click', function () {
                                        if ($(this).hasClass('cancel')) {
                                            chtml.find('.commentbody').data('emojioneArea').disable();
                                            $(this).removeClass('cancel')
                                        }
                                        else {
                                            chtml.find('.commentbody').data('emojioneArea').enable();
                                            $(this).addClass('cancel')
                                        }
                                    })
                                    chtml.find('.commentbody').emojioneArea(
                                        {
                                            autoHideFilters: true,
                                            useSprite: true,
                                            pickerPosition: "bottom",
                                            events: {
                                                keypress: function (e, o) {
                                                    if (o.originalEvent.charCode == 13) {
                                                        console.log('Enter');
                                                        if (!o.originalEvent.shiftKey) {
                                                            console.log('no shift');
                                                            o.originalEvent.returnValue = false;
                                                            let body = chtml.find('.commentbody').data("emojioneArea").getText();
                                                            if (body != null & body != 'undefined' & body != '') {
                                                                chtml.addClass('saving');
                                                                let data1 = JSON.stringify({ Body: XSS(escape(body)), Id: v1.id });
                                                                $.ajax({
                                                                    type: "POST",
                                                                    url: "Person.aspx/editcomentbyid",
                                                                    data: { data: data1 },
                                                                    headers: { headeraction: data1 },
                                                                    success: function (response) {
                                                                        var result = JSON.parse(response);
                                                                        if (result.status) {
                                                                            chtml.find('.commentbody').data("emojioneArea").disable();
                                                                            //$(chtml[0]).attr('id', 'comment' + result.data);
                                                                            chtml.removeClass('saving')
                                                                        }
                                                                    }
                                                                });
                                                            }
                                                            return false;

                                                        }
                                                        else {

                                                        }
                                                    }

                                                }
                                            }
                                        }
                                    );
                                    chtml.find('.commentbody').data('emojioneArea').disable();
                                }
                                else {
                                    chtml.find('.commentdel').remove()
                                    chtml.find('.commentedit').remove()
                                }
                                html.find('.postcommentscontainer').append(chtml);

                            });
                        }

                        //template = template.replaceAll('{{postimage}}', '');

                        if (v.UserId == sID) {
                            html.find('.editpost').on('click', function () {
                                var e = {};
                                e.Event = 'EditTimeline_Click';
                                if (e.Event == '_Click')
                                    return false;
                                e.Value = v.id;
                                BBPPostBack2(null, e);
                                return false;
                            })
                            html.find('.deletepost').on('click', function () {
                                var e = {}; e.Event = 'DeleteTimeline_Click';
                                if (e.Event == '_Click')
                                    return false;
                                e.Value = v.id; BBPPostBack2(null, e); return false;
                            })

                        }
                        else {
                            html.find('.mainpostaction').remove();
                        }

                        if (v.Attachments && v.Attachments.length > 0) {
                            let attcount = v.Attachments.length;
                            let cssClass = 'col-12';
                            if (attcount > 1)
                                cssClass = 'col-6';
                            $.each(v.Attachments, function (ii,vv) {
                                let atthtml = '<div class="' + cssClass + '"><img src="' + vv.URL + '" data-id="' + vv.id + '" data-parentid="' + vv.ParentID + '" class="img-fluid preview"></div>';
                                html.find('.attachment-container').append(atthtml);
                            });
                        }

                        $('#post-container').append(html);
                    })
                    loadingpost = false;

                    if (waypoint) {
                        waypoint.destroy();
                    }
                    var waypoint = new Waypoint({
                        element: document.getElementsByTagName('body'),
                        handler: function () {
                            if (!loadingpost & !postfinished)
                                getpost();
                        },
                        offset: 'bottom-in-view'
                    })
                });

            }

            $(function () {
                getpost();
                $.get(`api/media?type=images&count=9&sID=${userId}&isTestNet=${isTestNet}`, function (response) {
                    $.each(response, function (i, v) {
                        let temp = '<div class="col-md-4" ><figure><img data-id="' + v.id + '" data-parentid="' + v.ParentId +'" class="img-fluid preview" src="' + v.URL + '" alt="' + v.Title + '"></figure></div>'
                        $('#latest-images').append(temp);
                    })
                })
                $.get(`api/people/friends?sID=${userId}&isTestNet=${isTestNet}`, function (response) {
                    $('#friendscount').html(response.total);
                    $.each(response.result, function (i, v, a) {
                        let template = $('#friend-template').html().toString();
                        let p = '';
                        if (v.ProfilePicture.startsWith('<img'))
                            p = $(v.ProfilePicture).attr('src');
                        else {
                            p = v.ProfilePicture;
                        }
                        template = template.replaceAll('{{userpic}}', p);
                        template = template.replaceAll('{{user}}', v.FullName);
                        template = template.replaceAll('{{userid}}', v.Id);
                        $('#friends-container').append(template);
                    })
                })
            })

            $('body').on('click', 'img.preview', function () {
               let id = $(this).attr('data-id');
                let parentid = $(this).attr('data-parentid');
                //getMediaById()
                $('#contentpopup').modal('show');
                $('#contentpopup').addClass('loading');

                $('#contentpopup img.tosee').attr('src', '');
                $('#contentpopup img.userimage').attr('src', '');
                $('#contentpopup .username').html('');
                $('#contentpopup .time').html('');
                $('#contentpopup .commententrycontainer').html('<textarea class="mb-1 writecomment" rows="1" placeholder="Write a public comment."></textarea>');

                $('#contentpopup #comment-container').html('');

                
                $.get(`api/media/get-by-id?parentid=${parentid}&isTestNet=${isTestNet}`, function (response) {

                    let v = response.find(function (t) { return t.id == id; });

                    //if (!v)
                      //  v = response[0];
                    $('#contentpopup img.tosee').attr('src', response[0].URL);
                    $('#contentpopup img.userimage').attr('src', response[0].ProfilePicture);
                    $('#contentpopup .username').html(response[0].FullName);
                    $('#contentpopup .time').html(PrepareTime(response[0].PostedOn));
                    $('#contentpopup .likecount .count').html(response[0].Likes);
                    $('#contentpopup .viewcount .count').html(response[0].Views);

                    let emojin =  $('#contentpopup .writecomment').emojioneArea({
                        autoHideFilters: true,
                        useSprite: true,
                        pickerPosition: "bottom",
                        events: {
                            keypress: function (e, o) {
                                if (o.originalEvent.charCode == 13) {
                                    if (!o.originalEvent.shiftKey) {
                                        o.originalEvent.returnValue = false;
                                        let body = emojin.data("emojioneArea").getText();
                                        if (body != null & body != 'undefined' & body != '') {
                                            emojin.data("emojioneArea").setText('');
                                            let ctemplate = $('#comment-template').html().toString();
                                            var chtml = $(ctemplate);
                                            let pic = $('.new-comment .commenterpic').attr('src')

                                            chtml.find('.commenterpic').attr('src', pic);
                                            chtml.find('.commentername').html('<%= user.FullUserName()  %>');
                                            chtml.find('.commenttime').html(PrepareTime(new Date()));
                                            chtml.find('.commentbody').html(body?.replaceAll('\n', '<br\>'));
                                            $('#contentpopup #comment-container').append(chtml);
                                            chtml.addClass('saving');
                                            let data1 = JSON.stringify({ Body: XSS(escape(body)), ParentID: response[0].id });
                                            $.ajax({
                                                type: "POST",
                                                url: "Person.aspx/comment",
                                                data: { data: data1 },
                                                headers: { headeraction: data1 },
                                                success: function (response) {
                                                    var result = JSON.parse(response);
                                                    if (result.status) {
                                                        $(chtml[0]).attr('id', 'comment' + result.data);
                                                        chtml.removeClass('saving')
                                                    }
                                                }
                                            });
                                        }
                                        return false;

                                    }
                                    else {

                                    }
                                }

                            }
                        }
                    });

                    $('#contentpopup .btnlike').on('click', function () {
                        let param = 'upvote|' + v.id + "|comment1";
                        $('#contentpopup .likecount').addClass('saving')
                        $.ajax({
                            type: "POST",
                            url: "Person.aspx/voting",
                            data: { data: param },
                            headers: { headeraction: param },
                            success: function (response) {
                                var result = JSON.parse(response);
                                if (result.status.status) {
                                    $('#contentpopup .likecount .count').html(result.nUpvotes);
                                    if (result.nUpvotes > 0) {
                                        $('#contentpopup .likecount').show()
                                    }
                                    else {
                                        $('#contentpopup .likecount').hide()
                                    }
                                    $('#contentpopup .likecount').removeClass('saving')
                                }
                            }
                        });

                    })

                    $('#contentpopup .btncomment').on('click', function () {
                        //console.log(emojin[0].getBoundingClientRect().top);
                        //console.log($(emojin[0].getBoundingClientRect().top).offset().top);
                        emojin.data("emojioneArea").setFocus();
                    });
                    if (v.Comments && v.Comments.length > 0) {
                        $.each(v.Comments, function (i, v1, a) {
                            let ctemplate = $('#comment-template').html().toString();
                            var chtml = $(ctemplate);
                            let p1 = '';
                            if (v1.ProfilePicture.startsWith('<img'))
                                p1 = $(v1.ProfilePicture).attr('src');
                            else {
                                p1 = v1.ProfilePicture;
                            }
                            chtml.find('.commenterpic').attr('src', p1);
                            chtml.find('.commentername').html(v1.FullName);
                            chtml.find('.commenttime').addClass('small d-block');
                            chtml.find('.commenttime').html(PrepareTime(v1.PostedOn));
                            chtml.find('.commentbody').html(v1.Body?.replaceAll('\n', '<br\>'));

                            //Count.nUpvotes nDownvotes
                            chtml.find('.commentview .likecount .count').html(v1.Count.nUpvotes);
                            if (v1.Count.nUpvotes > 0) {
                                chtml.find('.commentview .likecount').show()
                            }
                            else {
                                chtml.find('.commentview .likecount').hide()
                            }
                            chtml.find('.commentview .dislikecount .count').html(v1.Count.nDownvotes);
                            if (v1.Count.nDownvotes > 0) {
                                chtml.find('.commentview .dislikecount').show()
                            }
                            else {
                                chtml.find('.commentview .dislikecount').hide()
                                chtml.find('.commentview .likecount').addClass('no-dislike')//'right', '10px !important');
                            }
                            $(chtml[0]).attr('id', 'comment' + v1.id);
                            chtml.find('.commentlike').addClass('small')
                            chtml.find('.commentdis').addClass('small')
                            chtml.find('.commentdel').addClass('small')
                            chtml.find('.commentedit').addClass('small')
                            chtml.find('.commentlike').on('click', function () {
                                let param = 'upvote|' + v1.id + "|comment1";
                                chtml.addClass('saving')
                                $.ajax({
                                    type: "POST",
                                    url: "Person.aspx/voting",
                                    data: { data: param },
                                    headers: { headeraction: param },
                                    success: function (response) {
                                        var result = JSON.parse(response);
                                        if (result.status.status) {
                                            chtml.find('.commentview .likecount .count').html(result.nUpvotes);
                                            if (result.nUpvotes > 0) {
                                                chtml.find('.commentview .likecount').show()
                                            }
                                            else {
                                                chtml.find('.commentview .likecount').hide()
                                            }
                                            chtml.find('.commentview .dislikecount .count').html(result.nDownvotes);
                                            if (result.nDownvotes > 0) {
                                                chtml.find('.commentview .dislikecount').show()
                                            }
                                            else {
                                                chtml.find('.commentview .dislikecount').hide()
                                                chtml.find('.commentview .likecount').addClass('no-dislike')//'right', '10px !important');
                                            }
                                            chtml.removeClass('saving')
                                        }
                                    }
                                });

                            })
                            chtml.find('.commentdis').on('click', function () {
                                let param = 'downvote|' + v1.id;
                                chtml.addClass('saving')
                                $.ajax({
                                    type: "POST",
                                    url: "Person.aspx/voting",
                                    data: { data: param },
                                    headers: { headeraction: param },
                                    success: function (response) {
                                        var result = JSON.parse(response);
                                        if (result.status) {
                                            chtml.find('.commentview .likecount .count').html(result.nUpvotes);
                                            if (result.nUpvotes > 0) {
                                                chtml.find('.commentview .likecount').show()
                                            }
                                            else {
                                                chtml.find('.commentview .likecount').hide()
                                            }
                                            chtml.find('.commentview .dislikecount .count').html(result.nDownvotes);
                                            if (result.nDownvotes > 0) {
                                                chtml.find('.commentview .dislikecount').show()
                                            }
                                            else {
                                                chtml.find('.commentview .dislikecount').hide()
                                                chtml.find('.commentview .likecount').addClass('no-dislike')//'right', '10px !important');
                                            }
                                            chtml.removeClass('saving')
                                        }
                                    }
                                });

                            })
                            if (v1.UserId == sID) {
                                chtml.find('.commentdel').on('click', function () {
                                    chtml.addClass('saving')
                                    $.ajax({
                                        type: "POST",
                                        url: "Person.aspx/deletecomentbyid",
                                        data: { data: v1.id },
                                        headers: { headeraction: v1.id },
                                        success: function (response) {
                                            var result = JSON.parse(response);
                                            if (result.status) {
                                                chtml.remove();
                                            }
                                        }
                                    });
                                })
                                chtml.find('.commentedit').on('click', function () {
                                    if ($(this).hasClass('cancel')) {
                                        chtml.find('.commentbody').data('emojioneArea').disable();
                                        $(this).removeClass('cancel')
                                    }
                                    else {
                                        chtml.find('.commentbody').data('emojioneArea').enable();
                                        $(this).addClass('cancel')
                                    }
                                })
                                chtml.find('.commentbody').emojioneArea(
                                    {
                                        autoHideFilters: true,
                                        useSprite: true,
                                        pickerPosition: "bottom",
                                        events: {
                                            keypress: function (e, o) {
                                                if (o.originalEvent.charCode == 13) {
                                                    console.log('Enter');
                                                    if (!o.originalEvent.shiftKey) {
                                                        console.log('no shift');
                                                        o.originalEvent.returnValue = false;
                                                        let body = chtml.find('.commentbody').data("emojioneArea").getText();
                                                        if (body != null & body != 'undefined' & body != '') {
                                                            chtml.addClass('saving');
                                                            let data1 = JSON.stringify({ Body: XSS(escape(body)), Id: v1.id });
                                                            $.ajax({
                                                                type: "POST",
                                                                url: "Person.aspx/editcomentbyid",
                                                                data: { data: data1 },
                                                                headers: { headeraction: data1 },
                                                                success: function (response) {
                                                                    var result = JSON.parse(response);
                                                                    if (result.status) {
                                                                        chtml.find('.commentbody').data("emojioneArea").disable();
                                                                        //$(chtml[0]).attr('id', 'comment' + result.data);
                                                                        chtml.removeClass('saving')
                                                                    }
                                                                }
                                                            });
                                                        }
                                                        return false;

                                                    }
                                                    else {

                                                    }
                                                }

                                            }
                                        }
                                    }
                                );
                                chtml.find('.commentbody').data('emojioneArea').disable();
                            }
                            else {
                                chtml.find('.commentdel').remove()
                                chtml.find('.commentedit').remove()
                            }
                            $('#contentpopup #comment-container').append(chtml);

                        });
                    }
                    $('#contentpopup').removeClass('loading');
                })
            })
            function PrepareTime(t) {
                return moment(t).fromNow();
            }

        </script>
    </div>
</asp:Content>
