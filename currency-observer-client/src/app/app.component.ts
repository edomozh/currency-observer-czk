import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  template: `
    <div style="text-align:center" class="content">
      <app-rate-checker></app-rate-checker>
    </div>
  `,
  styles: []
})
export class AppComponent {
  title = 'currency-observer-client';
}
