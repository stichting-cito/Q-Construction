export class ItemTypeUtilities {
    static isEmpty(html: string): boolean {
        if (html === '') {
            return true;
        }
        if (html.indexOf('img')) {
            return false;
        }
        const regex = /(<([^>]+)>)/ig;
        html = html.replace(regex, '');
        return (html) ? html.trim() === '' : true;
    }
}
