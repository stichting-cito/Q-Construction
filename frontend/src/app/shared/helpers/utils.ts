export function strip(html: string): string {
    const regex = /(<([^>]+)>)/ig;
    html = html.replace('<br>', ' ');
    return html.replace(regex, '');
}

export function truncate(text: string, length: number, emptyText = ''): string {
    if (text && text.indexOf('<') !== -1) {
        text = strip(text);
    }
    if (!text || text.trim() === '') {
        return emptyText;
    } else if (text.length > length) {
        return  text.substring(0, length) + '...';

    } else { return text; }
}
