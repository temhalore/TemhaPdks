import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  standalone: true,
  imports: [RouterOutlet]
})
export class AppComponent {
  title = 'lore-pdks-web';

  constructor(private spinnerService: NgxSpinnerService) {}

  /**
   * Spinner'ı göstermek için kullanılır
   */
  showSpinner(): void {
    this.spinnerService.show();
  }

  /**
   * Spinner'ı gizlemek için kullanılır
   */
  hideSpinner(): void {
    this.spinnerService.hide();
  }
}
