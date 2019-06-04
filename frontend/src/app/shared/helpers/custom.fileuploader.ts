import { FileUploader, FileUploaderOptions, ParsedResponseHeaders } from 'ng2-file-upload/file-upload/file-uploader.class';
import { FileItem } from 'ng2-file-upload/file-upload/file-item.class';
import { Observable, Observer } from 'rxjs';

export class CustomFileUploader extends FileUploader {
    private attachmentId: string;

    constructor(url: string) {
        super({
            url,
            authToken: localStorage.getItem('id_token')
        });
    }
    public upload(fileItem: FileItem): Observable<string> {
        return new Observable((observer: Observer<string>) => {
            this.onSuccessItem = (fi: FileItem, response: string, status: number, headers: ParsedResponseHeaders) => {
                const attachmentId = JSON.parse(response).value.attachmentId;
                fi.remove();
                observer.next(attachmentId);
                observer.complete();
            };
            this.onWhenAddingFileFailed = () => {
                observer.next('');
                observer.complete();
            };

            fileItem.upload();
        });
    }
    public uploadFile(file: File): Observable<string> {
        const fileItem = new FileItem(this, file, this.options);
        this.queue.push(fileItem);
        return this.upload(fileItem);
    }
}
