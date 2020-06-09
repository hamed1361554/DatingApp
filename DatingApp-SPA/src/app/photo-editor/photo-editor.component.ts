import { UserService } from './../services/user.service';
import { AlertifyService } from './../services/alertify.service';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { FileUploader, FileItem } from 'ng2-file-upload';
import { Photo } from '../models/photo';
import { environment } from '../../environments/environment';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css'],
})
export class PhotoEditorComponent implements OnInit {
  @Input()
  photos: Photo[];
  @Output()
  getMemberPhotoChanged = new EventEmitter<string>();
  uploader: FileUploader;
  hasBaseDropZoneOver: boolean;
  hasAnotherDropZoneOver: boolean;
  response: string;
  baseUrl = environment.apiUrl;
  currentMain: Photo;

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url:
        this.baseUrl +
        'users/' +
        this.authService.decodedToken.nameid +
        '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
      additionalParameter: {
        issueId: this.authService.decodedToken.nameid
      }
    });

    this.hasBaseDropZoneOver = false;

    this.uploader.onBeforeUploadItem = (fileItem) => {
      console.log(fileItem);
    };

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo: Photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain,
        };

        this.photos.push(photo);
      }
    };

    this.uploader.onErrorItem = (item, response, status, headers) => {
      this.alertify.error(response);
    };

    this.uploader.onBeforeUploadItem = (item) => {
      console.log(item);
    };

    this.response = '';
    this.uploader.response.subscribe((res) => {
      this.response = res;
      console.log(res);
    });
  }

  setMainPhoto(photo: Photo) {
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id).subscribe(() => {
      this.currentMain = this.photos.filter(p => p.isMain === true).pop();
      this.currentMain.isMain = false;
      photo.isMain = true;
      this.getMemberPhotoChanged.emit(photo.url);
    }, error => {
      this.alertify.error(error);
    });
  }
}
