import { ElementRef, HostListener, Directive, OnInit } from '@angular/core';

@Directive({ selector: '[appAutosize]' })
export class AutosizeDirective implements OnInit {
  @HostListener('input', ['$event.target'])
  onInput(textArea: any) {
    this.adjust();
  }
  constructor(public element: ElementRef) {
  }
  ngOnInit() {
    this.adjust();
  }
  adjust() {
    this.element.nativeElement.style.height = 'auto';
    this.element.nativeElement.style.height = this.element.nativeElement.scrollHeight + 'px';
  }
}
