import { IImageHandler } from './../../sharedcomponents/editor/quill.interfaces';
import { Item } from './../../shared/model/model';
import { Observable, Observer } from 'rxjs';
import { ItemService } from './../../shared/services/item.service';
import { CustomFileUploader } from './custom.fileuploader';
import { environment } from './../../../environments/environment';
import { map } from 'rxjs/operators';
export class AddImageToItemHandler implements IImageHandler {
    constructor(private item: Item, private itemService: ItemService) { }

    handle(file: File): Observable<string> {
        const url = `${environment.api}/Items/${this.item.id}/attachment`;
        const uploader = new CustomFileUploader(url);
        return uploader.uploadFile(file)
            .pipe(map((attachmentId) => {
                if (!attachmentId) {
                    return '';
                }
                this.filesUploaded(attachmentId);
                return this.itemService.getAttachmentUrl(this.item.id, attachmentId, true);
            }));
    }

    private filesUploaded(attachmentId: string) {
        this.item.attachmentIds = (this.item.attachmentIds) ?
            this.item.attachmentIds.concat([attachmentId]) :
            [attachmentId];
        this.itemService.save(this.item);
    }
}

export class ImageToBase64Handler implements IImageHandler {
    handle(file: File): Observable<string> {
        return new Observable((observer: Observer<string>) => {
            try {
                const reader = new FileReader();
                reader.onload = ((theFile) => {
                    return (e: any) => {
                        observer.next(e.target.result);
                        observer.complete();
                    };
                })(file);
                reader.onerror = () => {
                    observer.next('');
                    observer.complete();
                };
                reader.onabort = () => {
                    observer.next('');
                    observer.complete();
                };
                reader.readAsDataURL(file);
            } catch (error) {
                observer.next('');
                observer.complete();
            }
        });
    }
}
