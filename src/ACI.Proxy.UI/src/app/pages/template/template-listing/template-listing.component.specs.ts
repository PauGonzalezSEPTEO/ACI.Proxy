import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TemplateListingComponent } from './template-listing.component';

describe('TemplateListingComponent', () => {
  let component: TemplateListingComponent;
  let fixture: ComponentFixture<TemplateListingComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [TemplateListingComponent]
    });
    fixture = TestBed.createComponent(TemplateListingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});