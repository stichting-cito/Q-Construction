import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
// import moment from 'moment/moment';
// import 'moment/locale/en-gb';
// import 'moment/locale/nl';

/**
 * This class represents the main application component. Within the @Routes annotation is the configuration of the
 * applications routes, configuring the paths for the lazy loaded components (HomeComponent, AboutComponent).
 */
@Component({
    selector: 'app-root',
    templateUrl: 'app.component.html'
})
export class AppComponent {
    constructor(translate: TranslateService) {
        let userLang = navigator.language.split('-')[0]; // use navigator lang if available
        userLang = /(nl|en)/gi.test(userLang) ? userLang : 'en';

        // Set the locale for dates and times (using momentjs)

        // this language will be used as a fallback when a translation isn't found in the current language
        translate.setDefaultLang('en');
        if (localStorage.getItem('selectedLanguage')) {
            userLang = localStorage.getItem('selectedLanguage');
        }
        // the lang to use, if the lang isn't available, it will use the current loader to get them
        translate.use(userLang);
    }
}

